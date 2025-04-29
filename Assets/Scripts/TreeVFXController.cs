using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;
public class TreeVFXController : MonoBehaviour
{

    [SerializeField]
    Gradient leavesGradient = new Gradient();
    [SerializeField]
    private VisualEffect visualEffect;

    private Color key0 = new Color(0.4619081f, 0.8490566f, 0.4795057f, 1f);
    private Color key1 = new Color(0f, 0.3144653f, 0.01347709f, 1f);


    [SerializeField]
    private bool fall;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //color = visualEffect.GetGradient("Color");

        leavesGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(key0, 0f),
                new GradientColorKey(Color.blue, 1f) // Nodo derecho
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
            );
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.R))
        {
            key0 = new Color(0.8509804f, 0.6565626f, 0.4627451f, 1f);
            key1 = Color.red;
        }
        else if(Input.GetKeyUp(KeyCode.G))
        {
            key0 = new Color(0.4619081f, 0.8490566f, 0.4795057f, 1f);
            key1 = new Color(0f, 0.3144653f, 0.01347709f, 1f);
        }
        else if (Input.GetKeyUp(KeyCode.B))
        {
            key0 = new Color(0.4627451f, 0.6101018f, 0.8509804f, 1f);
            key1 = Color.blue;
        }

        visualEffect.SetBool("Fall", fall);

        
        GradientColorKey[] colorKeys = leavesGradient.colorKeys;
        colorKeys[0].color = key0;
        colorKeys[colorKeys.Length - 1].color = key1;

        leavesGradient.colorKeys = colorKeys;

        visualEffect.SetGradient("Leaves Gradient", leavesGradient);
    }
}
