
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CognitiveServicesAuthorization;
using Microsoft.Bing.Speech;
// Import Wave of NAudio to use the MediaFoundationReader
using NAudio.Wave;
using NLayer.NAudioSupport;
using NReco.VideoConverter;

public class Program
{
    private const string Path = @"C:\Users\kliu7\Documents\Bitcamp2018\SpeechText.txt";

    private const string key = "138e3d1c4d8f493bbbd136c5d1c27dbc";

    /// <summary>
    /// Short phrase mode URL
    /// </summary>
    private static readonly Uri ShortPhraseUrl = new Uri(@"wss://speech.platform.bing.com/api/service/recognition");

    /// <summary>
    /// The long dictation URL
    /// </summary>
    private static readonly Uri LongDictationUrl = new Uri(@"wss://speech.platform.bing.com/api/service/recognition/continuous");

    /// <summary>
    /// A completed task
    /// </summary>
    private static readonly Task CompletedTask = Task.FromResult(true);

    /// <summary>
    /// Cancellation token used to stop sending the audio.
    /// </summary>
    private readonly CancellationTokenSource cts = new CancellationTokenSource();

    public static WindowsFormsApp1.Form1 formtest = new WindowsFormsApp1.Form1();
    public static void Main()
    {
        // Send a speech recognition request for the audio.

        formtest.Show();
        
        var lang = formtest.getLang();
        var lengt = formtest.getLength();

        var p = new Program();
        p.Run(@"C:\Users\kliu7\Documents\Bitcamp2018\output.wav", lang, "long".Equals(lengt) ? LongDictationUrl : ShortPhraseUrl, key).Wait();
    }

    /// <summary>
    /// Invoked when the speech client receives a partial recognition hypothesis from the server.
    /// </summary>
    /// <param name="args">The partial response recognition result.</param>
    /// <returns>
    /// A task
    /// </returns>
    public Task OnPartialResult(RecognitionPartialResult args)
    {
        //Console.WriteLine("--- Partial result received by OnPartialResult ---");

        // Print the partial response recognition hypothesis.
        //Console.WriteLine(args.DisplayText);

        //Console.WriteLine();

        return CompletedTask;
    }

    /// <summary>
    /// Invoked when the speech client receives a phrase recognition result(s) from the server.
    /// </summary>
    /// <param name="args">The recognition result.</param>
    /// <returns>
    /// A task
    /// </returns>
    public Task OnRecognitionResult(RecognitionResult args)
    {
        var response = args;
        Console.WriteLine();

        //Console.WriteLine("--- Phrase result received by OnRecognitionResult ---");

        // Print the recognition status.
        Console.WriteLine("*** Phrase Recognition Status = [{0}] *", response.RecognitionStatus);

        // create arrayList to save Texts.
        List<string> phrases = new List<string>();

        if (response.Phrases != null)
        {
            foreach (var result in response.Phrases)
            {
                // Print the recognition phrase display text.
                //Console.WriteLine("{0} (Confidence:{1})", result.DisplayText, result.Confidence);
                phrases.Add(result.DisplayText);    // place text in arrayList.
            }

            // Extract only final result.
            string text = phrases[phrases.Count - 1];
            Console.WriteLine(text);   // Print out only last phrase.

            // Output text to a .txt file.
            //File.WriteAllText(Path, text);
            using (StreamWriter file = new StreamWriter(Path, true))
            {
                file.WriteLine(text);
            }
        }

        Console.WriteLine();
        return CompletedTask;
    }

    /// <summary>
    /// Sends a speech recognition request to the speech service
    /// </summary>
    /// <param name="audioFile">The audio file.</param>
    /// <param name="locale">The locale.</param>
    /// <param name="serviceUrl">The service URL.</param>
    /// <param name="subscriptionKey">The subscription key.</param>
    /// <returns>
    /// A task
    /// </returns>
    public async Task Run(string audioFile, string locale, Uri serviceUrl, string subscriptionKey)
    {
        // create the preferences object
        var preferences = new Preferences(locale, serviceUrl, new CognitiveServicesAuthorizationProvider(subscriptionKey));

        // Create a a speech client
        using (var speechClient = new SpeechClient(preferences))
        {
            speechClient.SubscribeToPartialResult(this.OnPartialResult);
            speechClient.SubscribeToRecognitionResult(this.OnRecognitionResult);

            // create an audio content and pass it a stream.
            using (var audio = new FileStream(audioFile, FileMode.Open, FileAccess.Read))
            {
                var deviceMetadata = new DeviceMetadata(DeviceType.Near, DeviceFamily.Desktop, NetworkType.Ethernet, OsName.Windows, "1607", "Dell", "T3600");
                var applicationMetadata = new ApplicationMetadata("SampleApp", "1.0.0");
                var requestMetadata = new RequestMetadata(Guid.NewGuid(), deviceMetadata, applicationMetadata, "SampleAppService");

                await speechClient.RecognizeAsync(new SpeechInput(audio, requestMetadata), this.cts.Token).ConfigureAwait(false);
            }
        }
    }
}

