using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISandbox
{
    public interface IBoid
    {
        #region Variables

        #endregion

        #region Getters_Setters

        float Max_Velocity { get; }
        float Max_Steering_Acceleration { get; }

        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        Vector2 Steering { get; set; }

        #endregion

        #region Unity

        #endregion

        #region Custom

        #endregion
    }
}