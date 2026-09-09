using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{

    [SerializeField] private Image[] _itemIcon;
    [SerializeField] private Sprite _iconA;
    [SerializeField] private Sprite _iconB;


    void Start()
    {

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Test1();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Test2();
        }


    }
    private void Test1()
    {
        foreach (var item in _itemIcon)
        {
            item.gameObject.SetActive(true);
            item.sprite = _iconA;
        }
    }

    private void Test2()
    {
        foreach (var item in _itemIcon)
        {
            item.gameObject.SetActive(true);
            item.sprite = _iconB;
        }
    }


}
