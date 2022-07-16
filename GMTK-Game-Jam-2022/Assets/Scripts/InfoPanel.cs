using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    public static InfoPanel instance;

    public TextMeshProUGUI textField;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Show("Hello there!");
        }
    }

    public void Show(string text)
    {
        textField.SetText(text);

        animator.Play("InfoPanelShow", 0);
    }
}
