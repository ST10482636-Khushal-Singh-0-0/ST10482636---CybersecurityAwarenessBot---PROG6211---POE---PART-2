using System;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace CybersecurityAwarenessBot
{
    public partial class MainWindow : Window
    {
        // These are the missing variables causing the errors!
        private BotEngine _bot;
        private bool _isAwaitingName = true;

        public MainWindow()
        {
            // This line fixes the 'ChatScroll' error by connecting C# to the UI
            InitializeComponent();

            _bot = new BotEngine();
            DisplayHeader();
            PlayVoiceGreeting();
            AppendMessage("System", "Welcome to your personalized Cybersecurity Awareness Bot! Please enter your name:");
        }

        private void DisplayHeader()
        {
            string guardianArt = @"
 _______    __   __     _____    _______   ______    _________    _____    ___   __
/ _____ \  | |   | |   / ___ \   | ___  \  | ___ \   |___  __|   / ___ \   |   \ | |
| |        | |   | |  / /   \ \  | |  | |  | |  \ |     | |     / /   \ \  | |\ \| |
| | _____  | |   | |  | |___| |  | |__| |  | |  | |     | |     | |___| |  | | \ \ |
| | |__ |  | |   | |  | _____ |  | __  /   | |  | |     | |     | _____ |  | |  \ \|
| |   | |  | |   | |  | |   | |  | | \ \   | |  | |     | |     | |   | |  | |   | |
| |___| |  | |___| |  | |   | |  | |  | |  | |__/ |   __| |___  | |   | |  | |   | |
\_______/  \_______/  |_|   |_|  |_|  |_|  |_____/   |_______|  |_|   |_|  |_|   |_|
                                                  
 +++ MZANSI'S CYBERSECURITY AWARENESS ASSISTANT +++
";
            ChatOutput.Text += guardianArt + "\n===============================================================================\n\n";
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("greeting.wav");
                player.LoadAsync();
                player.Play();
            }
            catch (Exception ex)
            {
                AppendMessage("System", $"[Voice greeting could not be played: {ex.Message}]");
            }
        }

        // The button click event
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        // The Enter key event
        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessInput();
            }
        }

        // The main logic for handling messages
        private void ProcessInput()
        {
            string input = UserInputBox.Text;
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendMessage(_isAwaitingName ? "User" : _bot.UserName, input);
            UserInputBox.Clear();

            if (_isAwaitingName)
            {
                _bot.UserName = input.Trim();
                _isAwaitingName = false;
                AppendMessage("Guardian", $"Hello, {_bot.UserName}! I am ready to help. You can ask me about privacy, scams, passwords, or phishing.");
            }
            else
            {
                string response = _bot.ProcessInput(input);
                AppendMessage("Guardian", response);
            }

            // Keep the chat scrolled to the bottom
            ChatScroll.ScrollToEnd();
        }

        private void AppendMessage(string sender, string message)
        {
            ChatOutput.Text += $"[{sender.ToUpper()}]: {message}\n\n";
        }
    }
}