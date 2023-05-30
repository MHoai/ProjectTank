using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
    public abstract class State : MonoBehaviour
    {
        [HideInInspector] public int addTank = 0;
        public abstract State RunCurrentState(int m_TypeMob, Transform PlayerLocation, GameObject m_Instance, Transform m_MobSpawner, Transform m_MobGoTo, bool oneTankLeft);
    }
}