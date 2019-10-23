using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AISandbox
{
    public class Timer : MonoBehaviour
    {
        #region Variables
        public int m_minutes = 0;
        public int m_seconds = 30;

        public int m_initial_total_seconds;
        public int m_total_seconds;

        public Text m_text;
        #endregion

        #region Getters_Setters

        public static bool Reset { get; set; }

        #endregion

        #region Unity
        private void OnEnable()
        {
            m_initial_total_seconds = 60 * m_minutes + m_seconds;
            m_total_seconds = m_initial_total_seconds;

            StartCoroutine(RunTimer());
        }

        // Update is called once per frame
        private void Update()
        {
            m_minutes = m_total_seconds / 60;
            m_seconds = m_total_seconds % 60;

            if(m_total_seconds == 0)
            {
                Reset = true;
                m_total_seconds = m_initial_total_seconds;
            }

            m_text.text = string.Format("Time: {0}:{1}", m_minutes.ToString("D2"), m_seconds.ToString("D2"));
        }

        #endregion

        #region Custom
        private IEnumerator RunTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                m_total_seconds--;
            }
        }
        #endregion
    }
}