using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher
{
    public class SingleInstanceManager : IDisposable
    {
        private const string MutexName = "AudioSwitcher_SingleInstance_Mutex";
        private const string PipeName = "AudioSwitcher_Switch_Pipe";
        
        private Mutex _mutex;
        private bool _isFirstInstance;
        private CancellationTokenSource _cts;
        private Task _pipeServerTask;

        public bool IsFirstInstance => _isFirstInstance;
        public event EventHandler SwitchRequested;

        public bool TryCreate()
        {
            _mutex = new Mutex(true, MutexName, out _isFirstInstance);
            return _isFirstInstance;
        }

        public void StartServer()
        {
            if (!_isFirstInstance) return;
            
            _cts = new CancellationTokenSource();
            _pipeServerTask = Task.Run(() => ListenForSwitchRequest(_cts.Token));
        }

        private async Task ListenForSwitchRequest(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using var server = new NamedPipeServerStream(
                        PipeName, 
                        PipeDirection.In, 
                        1, 
                        PipeTransmissionMode.Message,
                        PipeOptions.Asynchronous);
                    
                    await server.WaitForConnectionAsync(token);
                    
                    using var reader = new StreamReader(server);
                    var message = await reader.ReadLineAsync();
                    
                    if (message == "SWITCH")
                    {
                        SwitchRequested?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                }
            }
        }

        public static bool NotifyRunningInstance()
        {
            try
            {
                using var client = new NamedPipeClientStream(
                    ".", 
                    PipeName, 
                    PipeDirection.Out,
                    PipeOptions.Asynchronous);
                
                client.Connect(1000);
                
                using var writer = new StreamWriter(client) { AutoFlush = true };
                writer.WriteLine("SWITCH");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _pipeServerTask?.Wait(1000);
            _cts?.Dispose();
            
            if (_mutex != null)
            {
                if (_isFirstInstance)
                {
                    _mutex.ReleaseMutex();
                }
                _mutex.Dispose();
            }
        }
    }
}
