using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignButtonFunction : MonoBehaviour
{
    [SerializeField] Button Button;
    [SerializeField] int Index;

    private void Start()
    {
        SceneSwitcher switcher = SceneSwitcher.Instance;

        if(switcher == null)
        {
            Debug.Log("Scene switcher null");
            return;
        }

        switch (Index)
        {
            case 0:
                {
                    Button.onClick.AddListener(switcher.Play);
                    break;
                }
            case 1:
                {
                    Button.onClick.AddListener(switcher.ExitToMenu);
                    break;
                }
            case 2:
                {
                    Button.onClick.AddListener(switcher.ExitApplication);
                    break;
                }
        }

    }
}
