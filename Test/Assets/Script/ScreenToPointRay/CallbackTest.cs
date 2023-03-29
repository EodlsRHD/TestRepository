using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CallbackTest : MonoBehaviour
{


    void Start()
    {
        Roop();
    }


    void Update()
    {
        Debug.Log("Update");
    }

    void callback(string _str)
    {
        Debug.Log(_str);
    }

    async void Roop()
    {
        while(true)
        {
            await Task.Run(() => { callback("Roop"); });
        }
    }
}
