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

        float MaxSpeed { get; }
        Vector2 Position { get; set; }
        Vector2 Steering { get; set; }
        Vector2 AltSteering { get; set; }
        Vector2 Velocity { get; set; }

        #endregion

        #region Unity

        #endregion

        #region Custom

        #endregion
    }
}