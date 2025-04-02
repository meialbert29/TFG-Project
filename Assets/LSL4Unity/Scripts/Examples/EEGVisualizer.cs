using Assets.LSL4Unity.Scripts.AbstractInlets;
using System.Collections.Generic;
using UnityEngine;

public class EEGVisualizer : InletFloatSamples
{
    public Transform targetTransform; // Objeto a escalar
    public float scaleMultiplier = 0.1f; // Ajuste de tamaño

    private Queue<float> eegBuffer = new Queue<float>(); // Almacena los últimos valores
    private int bufferSize = 30; // Número de muestras para analizar

    protected override void Process(float[] newSample, double timeStamp)
    {
        // Guardar los valores en el buffer
        eegBuffer.Enqueue(newSample[0]);

        // Mantener el buffer con un tamaño fijo
        if (eegBuffer.Count > bufferSize)
            eegBuffer.Dequeue();

        // Cuando el buffer esté lleno, analizar los datos
        if (eegBuffer.Count == bufferSize)
        {
            float dominantFrequency = AnalyzeEEG(eegBuffer.ToArray());
            string waveType = ClassifyBrainWave(dominantFrequency);

            Debug.Log($"Onda detectada: {waveType} ({dominantFrequency} Hz)");

            // Cambiar la escala del objeto basado en la onda cerebral
            AdjustObjectScale(waveType);
        }
    }

    // Ajusta la escala del objeto en función del estado cerebral
    private void AdjustObjectScale(string waveType)
    {
        Vector3 baseScale = Vector3.one; // Tamaño original

        switch (waveType)
        {
            case "Delta":
                targetTransform.localScale = baseScale * 0.5f; // Pequeño
                break;
            case "Theta":
                targetTransform.localScale = baseScale * 0.8f;
                break;
            case "Alpha":
                targetTransform.localScale = baseScale * 1.2f;
                break;
            case "Beta":
                targetTransform.localScale = baseScale * 1.5f;
                break;
            case "Gamma":
                targetTransform.localScale = baseScale * 2.0f; // Grande
                break;
            default:
                targetTransform.localScale = baseScale; // Tamaño normal
                break;
        }
    }

    // Determina la frecuencia dominante en las muestras EEG
    private float AnalyzeEEG(float[] samples)
    {
        // Por ahora, tomamos el promedio como estimación (luego se puede mejorar con FFT)
        float sum = 0;
        foreach (float value in samples) sum += value;
        return sum / samples.Length;
    }

    // Clasifica la onda cerebral según la frecuencia
    private string ClassifyBrainWave(float frequency)
    {
        if (frequency < 4) return "Delta";
        if (frequency >= 4 && frequency < 8) return "Theta";
        if (frequency >= 8 && frequency < 14) return "Alpha";
        if (frequency >= 14 && frequency < 30) return "Beta";
        return "Gamma";
    }
}
