using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestView : MonoBehaviour
{
    [SerializeField] TestData testData;

    int int_a;
    int int_b;
    string str_a;
    void Start()
    {
        int_a = testData.num_A;
        int_b = testData.num_B;
        str_a = testData.str_A;

        Debug.Log($"{int_a}" + "" + $"{int_b}" + " " + str_a);
    }
}
