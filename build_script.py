import subprocess
import os
import configparser
import zipfile

def clear_build_path_dir(build_path):
    if os.path.exists(build_path):
        for root, dirs, files in os.walk(build_path, topdown=False):
            for file in files:
                print(f"Removing {os.path.join(root, file)}")
                os.remove(os.path.join(root, file))
            for dir in dirs:
                print(f"Removing {os.path.join(root, dir)}")
                os.rmdir(os.path.join(root, dir))
        os.rmdir(build_path)
    os.makedirs(build_path)

def build_game(project_path, unity_path, log_path):
    command = [
        "powershell",
        "-Command",
        f'& \'{unity_path}\' -quit -batchmode -logFile \'{log_path}\' -projectPath \'{project_path}\' -executeMethod BuildScript.PerformBuild'
    ]

    result = subprocess.run(command, capture_output=True, text=True)

    print("STDOUT:\n", result.stdout)
    print("STDERR:\n", result.stderr)

    try:
        with open(log_path, "r") as f:
            print("\n=== Unity Log ===\n")
            print(f.read())
    except Exception as e:
        print(f"\nCouldn't read Unity log: {e}")
        
def make_steam_appid(build_path):
    with open(os.path.join(build_path, 'steam_appid.txt'), 'w+') as f:
        f.write("480")
        
def compress_build(build_path):
    print("zipping project")
    # Name the zip file same as the folder, with .zip extension
    zip_path = build_path.rstrip("\\/") + ".zip"
    
    if os.path.exists(zip_path):
        print(f"Deleting existing zip file: {zip_path}")
        os.remove(zip_path)

    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(build_path):
            for file in files:
                file_path = os.path.join(root, file)
                # Make the zip path relative to the build folder
                arcname = os.path.relpath(file_path, start=build_path)
                zipf.write(file_path, arcname)
    print(f"Compressed build to: {zip_path}")
    return zip_path
    
def upload_to_gdrive(zip_path, remote_folder="UnityBuilds"):
    file_name = os.path.basename(zip_path)
    remote_path = f"gdrive:{remote_folder}/{file_name}"

    # Replace the file in Drive
    result = subprocess.run(["rclone", "copyto", zip_path, remote_path, "--progress"], capture_output=True, text=True)

    print("=== Rclone Output ===")
    print("STDOUT:\n", result.stdout)
    print("STDERR:\n", result.stderr)

    if result.returncode == 0:
        print(f"Uploaded {file_name} to Google Drive at {remote_path}")
    else:
        print(f"Failed to upload {file_name}")

def main():
    config = configparser.ConfigParser()
    config.read(os.path.join(os.path.dirname(__file__), 'build_config.ini'))
    
    project_path = config.get('paths', 'project_path')
    unity_path = config.get('paths', 'unity_path')  
    log_path = config.get('paths', 'log_path')
    
    build_path = os.path.join(project_path, 'Builds', 'SteamBuild')
    
    clear_build_path_dir(build_path)
    build_game(project_path, unity_path, log_path)
    make_steam_appid(build_path)
    zip_path = compress_build(build_path)
    upload_to_gdrive(zip_path)
       

if __name__ == "__main__":
    main()
