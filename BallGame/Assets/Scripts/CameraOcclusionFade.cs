using System.Collections.Generic;
using UnityEngine;

public class CameraOcclusionFade : MonoBehaviour
{
    [Header("Link to the player")]
    public Transform player;

    [Header("On which layer are the objects that are obstacles ")]
    public LayerMask obstructionLayer;  
    // We take the layer mask from the inspector, for example, “Instruction".

    [Header("Transparency overlay")]
    [Range(0f, 1f)]
    public float fadeAlpha = 0.3f;
    // How much to make the wall translucent. The default is 0.3, so that the wall is slightly visible.

    [Header("Transition speed")]
    [Tooltip("The more, the faster the transparency changes.")]
    public float fadeSpeed = 6f;

    // Internal structures for storing the current state of "clogged" renderers 
    // and their original colors.
    private readonly Dictionary<Renderer, Material[]> _storedMats = new Dictionary<Renderer, Material[]>();
    private readonly Dictionary<Renderer, Color[]>    _originalColors = new Dictionary<Renderer, Color[]>();

    // The list of renderers that are in the frame hit by the ray (Raycast) from the camera to the player
    private List<Renderer> _currentHits  = new List<Renderer>();
    private List<Renderer> _previousHits = new List<Renderer>();

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        if (_cam == null)
        {
            Debug.LogError("CameraOcclusionFade should be hanging on an object with a Camera component!");
        }
    }

    private void LateUpdate()
    {
        if (player == null || _cam == null) return;

        // 1) Calculate the direction and distance from the camera to the player
        Vector3 camPos    = _cam.transform.position;
        Vector3 playerPos = player.position;
        Vector3 dir       = playerPos - camPos;
        float   dist      = dir.magnitude;

        // 2) Making a RaycastAll on the obstructionLayer layer
        RaycastHit[] hits = Physics.RaycastAll(
            origin: camPos,
            direction: dir.normalized,
            maxDistance: dist,
            layerMask: obstructionLayer
        );

        // 3) Let's create a list of current hits (Renderers)
        _currentHits.Clear();
        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                // If it's not the player himself (just in case), add to the list
                if (hit.transform != player && !_currentHits.Contains(rend))
                {
                    _currentHits.Add(rend);
                }
            }
        }

        // 4) For all renderers that were "numb" before this frame,
        // but now they are not in _currentHits, we need to return the original material/color to them.
        foreach (var rend in _previousHits)
        {
            if (!_currentHits.Contains(rend))
            {
                RestoreRenderer(rend);
            }
        }

        // 5) For all the current ones that fall under the beam, we gradually "fade" (make it translucent).
        foreach (var rend in _currentHits)
        {
            FadeOutRenderer(rend);
        }

        // 6) Updating the list of "who was in the previous frame"
        _previousHits.Clear();
        _previousHits.AddRange(_currentHits);
    }

    // Changes this Renderer's materials and colors to translucent (fadeAlpha)
    private void FadeOutRenderer(Renderer rend)
    {
        // If we haven't saved his original materials/colors yet, we're doing it now:
        if (!_storedMats.ContainsKey(rend))
        {
            // 1) We keep a link to an array of original materials (a copy)
            Material[] matsCopy = rend.materials;
            _storedMats[rend]   = matsCopy;

            // 2) We keep the original colors of each material
            Color[] origCols = new Color[matsCopy.Length];
            for (int i = 0; i < matsCopy.Length; i++)
            {
                origCols[i] = matsCopy[i].color;
            }
            _originalColors[rend] = origCols;
        }

        // 3) Now we smoothly change each material (in the instance of rend.materials) to translucent.
        Material[] instanceMats = rend.materials;
        for (int i = 0; i < instanceMats.Length; i++)
        {
            Material mat = instanceMats[i];
            // We are switching the material to Fade (Standard Shader) mode —
            // so that the alpha channel starts working.
            ChangeRenderMode(mat, BlendMode.Fade);

            // We take the current color and gradually reduce its alpha to fadeAlpha
            Color currentColor = mat.color;
            float targetAlpha  = fadeAlpha;
            float newAlpha     = Mathf.MoveTowards(
                currentColor.a, 
                targetAlpha, 
                fadeSpeed * Time.deltaTime
            );

            currentColor.a = newAlpha;
            mat.color      = currentColor;
        }

        // 4) We substitute the received "instanced" materials back into the Renderer
        rend.materials = instanceMats;
    }

    // Restores the Renderer to its original materials/colors before we made them translucent
    private void RestoreRenderer(Renderer rend)
    {
        if (!_storedMats.ContainsKey(rend)) return;

        // Getting the returned arrays of materials and colors
        Material[] origMats   = _storedMats[rend];
        Color[]    origColors = _originalColors[rend];

       // 1) For each material in the copy, we return its old parameters
        for (int i = 0; i < origMats.Length; i++)
        {
            Material mat = origMats[i];
           // Return the material to Opaque mode (so that it becomes completely opaque again)
            ChangeRenderMode(mat, BlendMode.Opaque);

        // Restoring the color (from the previous Alpha = 1)
            mat.color = origColors[i];
        }

        // 2) We substitute back the array of original materials in the Renderer
        rend.materials = origMats;

        
        _storedMats.Remove(rend);
        _originalColors.Remove(rend);
    }

    // This enum and method are taken according to Unity's recommendations for switching the Standard Shader 
    // between Opaque, Cutout, Fade and Transparent modes
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;

            case BlendMode.Cutout:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 2450;
                break;

            case BlendMode.Fade:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;

            case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
        }
    }
}
