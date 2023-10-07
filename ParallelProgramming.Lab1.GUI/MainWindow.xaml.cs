using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ParallelProgramming.Lab1.App;

namespace ParallelProgramming.Lab1.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _previousText = string.Empty;
        private readonly ProducerConsumerManager<char> _manager = new(16);
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        public MainWindow()
        {
            InitializeComponent();
            InputBox.TextChanged += InputBoxTextChanged;
            ProducerLauncher(_cancellationTokenSource.Token);
            Closed += OnWindowClosed;
        }

        private void InputBoxTextChanged(object e, TextChangedEventArgs args)
        {
            var box = e as TextBox;
            var newText = box?.Text;
            if (string.IsNullOrEmpty(newText) || newText.Length <= _previousText.Length) return;
            // ReSharper disable once UseIndexFromEndExpression
            var ch = newText[newText.Length - 1];
            _manager.Produce(ch);
        }

        private void OnWindowClosed(object? e, EventArgs a)
        {
            _cancellationTokenSource.Cancel();
        }
        
        [MTAThread]
        private void ProducerLauncher(CancellationToken token)
        {
            var digitDetector = new Predicate<char>(char.IsDigit);
            var symbolDetector = new Predicate<char>(ch => !char.IsLetterOrDigit(ch));
            var alphabetDetector = new Predicate<char>(char.IsLetter);

            Thread Fabric(Predicate<char> detector, TextBlock block)
            {
                return new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        var consumeRes = _manager.Consume(detector);
                        if (consumeRes.Item2)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                block.Text += consumeRes.Item1;
                            });
                        }
                    }
                });
            }

            var consumer1 = Fabric(digitDetector, ConsumerBlock2);
            var consumer2 = Fabric(symbolDetector, ConsumerBlock1);
            var consumer3 = Fabric(alphabetDetector, ConsumerBlock3);

            consumer1.Start();
            consumer2.Start();
            consumer3.Start();
        }
    }
}