using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 20f;
    public float fadeDuration = 1f;
    public Text pointsText;
    private Color originalColor;

    void Start()
    {
        originalColor = pointsText.color;
        StartCoroutine(FadeAndMove());
    }

    IEnumerator FadeAndMove()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;

        while (timer < fadeDuration)
        {
            // Movimiento hacia arriba
            transform.position = startPos + Vector3.up * floatSpeed * (timer / fadeDuration);

            // Desvanecimiento
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            pointsText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Elimina el texto al terminar
    }
}
