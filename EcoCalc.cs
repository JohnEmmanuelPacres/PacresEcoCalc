using Spectre.Console;
using System.Text;
using NAudio.Wave;
using System.Threading;

namespace Pacres_EcoCalc
{
    public class EcoCalc
    {
        private static IWavePlayer? outputDevice;
        private static AudioFileReader? audioFile;
        private static Thread musicThread;
        private static bool playMusic = true;
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                AnsiConsole.Status()
                    .Start("Initiating...", ctx =>
                    {
                        AnsiConsole.MarkupLine("[yellow]Waking up..[/]");
                        Thread.Sleep(1000);

                        ctx.Status("[green]Starting...[/]");
                        ctx.Spinner(Spinner.Known.Clock);
                        ctx.SpinnerStyle(Style.Parse("green"));

                        AnsiConsole.MarkupLine("[cyan]\nDoing some more work...[/]");
                        Thread.Sleep(2000);
                    });
                AnsiConsole.Clear();

                musicThread = new Thread(() => PlayMusic("ecocalc_bgm.wav"));
                musicThread.IsBackground = true;
                musicThread.Start();

                Title title = new Title();
                title.display();
                Console.ReadKey();

                AnsiConsole.Clear();
                Menu menu = new Menu();
                menu.display();
            }
            catch (FormatException ex)
            {
                AnsiConsole.MarkupLine($"A [red]FormatException[/] has occurred\n[red]Error: {ex.Message}[/]\nProgram terminating...");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }
            catch (InvalidOperationException ex)
            {
                AnsiConsole.MarkupLine($"An [red]InvalidOperationException[/] has occurred\n[red]Error: {ex.Message}[/]\nProgram terminating...");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
            }
            catch (NullReferenceException ex)
            {
                AnsiConsole.MarkupLine("[red]A null reference exception occurred![/]");
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                Console.ReadKey();
                AnsiConsole.MarkupLine("[yellow slowblink]\nPress any keys to continue...[/]");
                Menu menu = new Menu();
                menu.display();
            }
            finally
            {
                AnsiConsole.Clear();
                var font = FigletFont.Load("3d.flf");
                AnsiConsole.Write(new FigletText(font, "\nGood bye!").Centered().Color(Color.Green));
                AnsiConsole.MarkupLine("\n[lime bold] Thank you for using the program [/]");
                AnsiConsole.MarkupLine("\n[lime bold] Program terminating... [/]");
                AnsiConsole.MarkupLine("\n[lime bold] Program successfully terminated... [/]");
                StopMusic();
                Console.ReadKey();
            }
        }
        public static void PlayMusic(string filepath)
        {
            do
            {
                if (!playMusic) break;

                outputDevice?.Dispose();
                audioFile?.Dispose();
                audioFile = new AudioFileReader(filepath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while(outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing && playMusic)
                {
                    Thread.Sleep(100);
                }
            }
            while (playMusic);
        }
        public static void StopMusic()
        {
            playMusic = false;
            outputDevice?.Stop();
            outputDevice?.Dispose();
            outputDevice = null;

            audioFile?.Dispose();
            audioFile = null;

            musicThread?.Join();
        }
    }
}