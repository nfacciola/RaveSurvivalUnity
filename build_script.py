import subprocess
import os
import configparser

def clear_build_path_dir(build_path):
    if os.path.exists(build_path):
        for root, dirs, files in os.walk(build_path, topdown=False):
            for file in files:
                os.remove(os.path.join(root, file))
            for dir in dirs:
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
    with open(os.path.join(os.path.dirname(build_path), 'steam_appid.txt'), 'w+') as f:
        f.write("480")

def main():
    config = configparser.ConfigParser()
    config.read(os.path.join(os.path.dirname(__file__), 'build_config.ini'))
    
    project_path = config.get('paths', 'project_path')
    unity_path = config.get('paths', 'unity_path')  
    log_path = config.get('paths', 'log_path')
    
    build_path = os.path.join(os.path.dirname(project_path), 'Builds', 'SteamBuild')
    
    clear_build_path_dir(build_path)
    build_game(project_path, unity_path, log_path)
    make_steam_appid(build_path)
    
    
    
    

if __name__ == "__main__":
    main()
