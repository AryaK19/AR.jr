using System.IO;
using HuggingFace.API;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;
using System.Collections;



public class SpeechRecognitionTest : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject plainFinder;

    private ContentPositioningBehaviour script;


    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start()
    {
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;
    }

    private void Update()
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples)
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        if (!recording) // Check if not already recording
        {
            text.color = Color.white;
            text.text = "Recording...";
            startButton.interactable = false;
            stopButton.interactable = true;
            clip = Microphone.Start(null, false, 10, 44100);
            recording = true;
        }
    }

    private IEnumerator StopRecordingCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // Add a short delay

        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private void StopRecording()
    {
        StartCoroutine(StopRecordingCoroutine());
    }

    private void SendRecording()
    {
        text.color = Color.yellow;
        text.text = "Sending...";
        stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            text.color = Color.white;
            text.text = response;
            onDisplay(response);
            startButton.interactable = true;
            
        }, error => {
            text.color = Color.red;
            text.text = error;
            startButton.interactable = true;
        });
    }

    public void onDisplay(string text)
    {
        script = plainFinder.GetComponent<ContentPositioningBehaviour>();
        GameObject currentObject = script.AnchorStage.gameObject;

        if (text.ToLower().Contains("apple") || text.ToLower().Remove(text.Length - 1) == "apple")
        {
            if (!currentObject.CompareTag("apple"))
            {
                GameObject newApple = GameObject.FindGameObjectWithTag("apple");
                script.AnchorStage = newApple.GetComponent<AnchorBehaviour>();
            }
        }
        else if (text.ToLower().Contains("banana") || text.ToLower().Remove(text.Length - 1) == "banana")
        {
            if (!currentObject.CompareTag("banana"))
            {
                GameObject newBanana = GameObject.FindGameObjectWithTag("banana");
                script.AnchorStage = newBanana.GetComponent<AnchorBehaviour>();
            }
        }
    }



    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}
