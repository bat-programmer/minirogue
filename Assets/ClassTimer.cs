using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassTimer : MonoBehaviour
{
    public float timerRestante = 100;
    private bool timerOn = false;
    // Start is called before the first frame update
    void Start()
    {
        timerOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        TimerCheck();
    }

    private void TimerCheck()
    {
        if (timerOn)
        {
            timerRestante -= Time.deltaTime;
            if (timerRestante <= 0)
            {
                timerOn = false;
                print("Se acabo el tiempo");
                timerRestante = 0;
            }
            else
            {
               // print("Tiempo restante: " + timerRestante);
            }
        }
    }
}
