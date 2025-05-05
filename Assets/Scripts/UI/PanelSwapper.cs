using System.Collections.Generic;
using UnityEngine;

namespace RaveSurvival
{
    public class PanelSwapper : MonoBehaviour
    {
        // List of panels to manage
        public List<Panel> panels;

        /// <summary>
        /// Activates multiple panels based on the provided indices.
        /// Panels with indices in the list will be activated, others will be deactivated.
        /// </summary>
        /// <param name="indices">List of indices of panels to activate</param>
        public void SetMultiplePanels(List<int> indices)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                // Activate the panel if its index is in the list, otherwise deactivate it
                if (indices.Contains(i))
                {
                    panels[i].gameObject.SetActive(true);
                }
                else
                {
                    panels[i].gameObject.SetActive(false); 
                }
            }
        }

        /// <summary>
        /// Activates a single panel based on the provided index.
        /// All other panels will be deactivated.
        /// </summary>
        /// <param name="index">Index of the panel to activate</param>
        public void SetActivePanel(int index)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                // Activate the panel if its index matches the provided index, otherwise deactivate it
                panels[i].gameObject.SetActive(i == index);
            }
        }

        /// <summary>
        /// Unity's Start method, called before the first frame update.
        /// Ensures the first panel in the list is active by default if there are panels.
        /// </summary>
        void Start()
        {
            if (panels.Count > 0)
            {
                SetActivePanel(0); // Activate the first panel by default
            }
        }
    }
}
