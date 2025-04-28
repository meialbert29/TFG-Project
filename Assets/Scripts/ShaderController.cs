using UnityEngine;
using UnityEngine.VFX;
public class ShaderController : MonoBehaviour
{
    private bool fall;
    public Gradient color;
    public VisualEffect visualEffect;

    private Color startColor = new Color(0.4619081f, 0.8490566f, 0.4795057f, 1f);
    private Color endColor = new Color(0f, 0.3144653f, 0.01347709f, 1f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fall = visualEffect.GetBool("Fall");
        color = visualEffect.GetGradient("Color");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!fall)
            {
                visualEffect.SetBool("Fall", true);
            }
            else
            {
                visualEffect.SetBool("Fall", false);
            }

            fall = visualEffect.GetBool("Fall");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            
        }
    }
}
