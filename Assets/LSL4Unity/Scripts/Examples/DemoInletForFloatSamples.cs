using System.Collections;
using UnityEngine;
using System.Linq;
using MathNet.Numerics.IntegralTransforms; // Para FFT
using System.Numerics;
using Assets.LSL4Unity.Scripts.AbstractInlets;
using System.Collections.Generic;
using System;

namespace Assets.LSL4Unity.Scripts.Examples
{

    /// <summary>
    /// Example that works with the Resolver component.
    /// This script waits for the resolver to resolve a Stream which matches the Name and Type.
    /// See the base class for more details. 
    /// 
    /// The specific implementation should only deal with the moment when the samples need to be pulled
    /// and how they should processed in your game logic
    ///
    /// </summary>
    public class DemoInletForFloatSamples : InletFloatSamples
    {
        //public Transform targetTransform; // Opcional: Visualizar el estado cerebral
        public int samplingRate = 256; // Frecuencia de muestreo del EEG (ajústala según el dispositivo)
        private int bufferSize = 256; // Número de muestras usadas para el análisis de frecuencia
        private Queue<float> eegBuffer = new Queue<float>(); // Buffer circular de muestras EEG

        protected override void Process(float[] newSample, double timeStamp)
        {
            // Asumiendo que newSample[0] contiene la señal EEG del primer canal
            eegBuffer.Enqueue(newSample[0]);

            // Mantener el tamaño del buffer
            if (eegBuffer.Count > bufferSize)
                eegBuffer.Dequeue();

            // Solo procesar cuando el buffer está lleno
            if (eegBuffer.Count == bufferSize)
            {
                float dominantFrequency = AnalyzeEEG(eegBuffer.ToArray());
                string waveType = ClassifyBrainWave(dominantFrequency);
                Debug.Log($"Onda dominante: {waveType} ({dominantFrequency} Hz)");
            }
        }

        private float AnalyzeEEG(float[] samples)
        {
            int N = samples.Length;
            Complex[] fftInput = new Complex[N];

            // Convertir la señal en números complejos
            for (int i = 0; i < N; i++)
                fftInput[i] = new Complex(samples[i], 0);

            // Aplicar FFT
            Fourier.Forward(fftInput, FourierOptions.NoScaling);

            // Obtener las magnitudes de frecuencia
            float[] magnitudes = fftInput.Select(c => (float)c.Magnitude).ToArray();

            // Encontrar la frecuencia dominante
            int maxIndex = Array.IndexOf(magnitudes, magnitudes.Max());
            float frequencyResolution = (float)samplingRate / N;
            return maxIndex * frequencyResolution;
        }

        private string ClassifyBrainWave(float frequency)
        {
            if (frequency < 4) return "Delta";
            if (frequency < 8) return "Theta";
            if (frequency < 14) return "Alpha";
            if (frequency < 30) return "Beta";
            return "Gamma";
        }
    }
}