using UnityEngine;

public class RainController : MonoBehaviour
{
    private ParticleSystem _rain;
    private CloudsController _cc;
    public GeneralController _gc;

    void Start()
    {
        _rain = GetComponent<ParticleSystem>();
        _cc = FindAnyObjectByType<CloudsController>();
        //_gc = FindAnyObjectByType<GeneralController>();

        if( _rain == null || _cc == null || _gc == null)
        {
            Debug.Log("Error in Rain Controller");
            return;
        }

        rainStop();
    }

    // Update is called once per frame
    void Update()
    {
        //if (_cc.skyIsChanging())
        //{
        //    rainStart();
        //}

    }

    public void rainStart()
    {
        _rain.Play();
    }

    public void rainStop()
    {
        _rain.Stop();
    }
}
