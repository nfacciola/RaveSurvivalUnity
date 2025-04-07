using System.Collections.Generic;
using UnityEngine;

public class PanelSwapper : MonoBehaviour
{
    public List<Panel> panels;

    public void SetMultiplePanels(List<int> indices)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            if(indices.Contains(i))
            {
                panels[i].gameObject.SetActive(true);
            }
            else
            {
                panels[i].gameObject.SetActive(false); 
            }
        }
    }

    public void SetActivePanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].gameObject.SetActive(i == index);
        }
    }

    void Start()
    {
        if (panels.Count > 0)
        {
            SetActivePanel(0);
        }
    }
}
