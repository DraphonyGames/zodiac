namespace Assets.Scripts.Camera
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// The class for auto transparency of objects between camera and hero
    /// </summary>
    public class AutoTransparent : MonoBehaviour
    {
        /// <summary>
        /// the target transparancy for objects
        /// </summary>
        private const float TargetTransparancy = 0.7f;

        /// <summary>
        /// returns to 100% in 0.1 sec
        /// </summary>
        private const float FallOff = 0.1f;

        /// <summary>
        /// store shader to reset it later
        /// </summary>
        private Shader _oldShader = null;

        /// <summary>
        /// store color to reset it later
        /// </summary>
        private Color _oldColor = Color.black;

        /// <summary>
        /// the current transparancy for objects
        /// </summary>
        private float _transparency = 0.7f;

        /// <summary>
        /// Sets the object to transparent
        /// Called every frame, if object is in the way
        /// </summary>
        public void BeTransparent()
        {
            // reset the transparency;
            _transparency = TargetTransparancy;

            if (_oldShader == null)
            {
                // Save the current shader
                _oldShader = renderer.material.shader;
                _oldColor = renderer.material.color;
                renderer.material.shader = Shader.Find("Transparent/Diffuse");
            }
        }

        /// <summary>
        /// fade out the trancperancy
        /// </summary>
        public void Update()
        {
            if (_transparency < 1.0f)
            {
                Color c = renderer.material.color;
                c.a = _transparency;
                renderer.material.color = c;
            }
            else
            {
                // Reset the shader
                renderer.material.shader = _oldShader;
                renderer.material.color = _oldColor;

                // And remove this script
                Destroy(this);
            }

            _transparency += ((1.0f - TargetTransparancy) * Time.deltaTime) / FallOff;
        }
    }
}