using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SSMT
{
    // 进程项数据模型
    public class ProcessItem : INotifyPropertyChanged
    {
        private int _index;
        public int Index
        {
            get => _index;
            set { _index = value; OnPropertyChanged(); }
        }

        private string _processPath;
        public string ProcessPath
        {
            get => _processPath;
            set { _processPath = value; OnPropertyChanged(); }
        }

        private string _arguments;
        public string Arguments
        {
            get => _arguments;
            set { _arguments = value; OnPropertyChanged(); }
        }

        private int _delay;
        public int Delay
        {
            get => _delay;
            set { _delay = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // 启动配置数据模型
    public class LaunchConfig
    {
        public string Name { get; set; }
        public ObservableCollection<ProcessItem> ProcessItems { get; set; } = new ObservableCollection<ProcessItem>();
        public DateTime LastModified { get; set; } = DateTime.Now;

        [JsonIgnore]
        public string FilePath { get; set; }
    }

    // 配置管理器
    public class ProcessConfigManager
    {
        private static readonly string ConfigsFolder = Path.Combine(GlobalConfig.SSMTCacheFolderPath, "ProcessConfigs");
        private ObservableCollection<LaunchConfig> _configs = new ObservableCollection<LaunchConfig>();

        public event Action<LaunchConfig> CurrentConfigChanged;

        private LaunchConfig _currentConfig;
        public LaunchConfig CurrentConfig
        {
            get => _currentConfig;
            set
            {
                _currentConfig = value;
                CurrentConfigChanged?.Invoke(value);
            }
        }

        public ProcessConfigManager()
        {
            InitializeConfigs();
        }

        private void InitializeConfigs()
        {
            // 确保配置文件夹存在
            Directory.CreateDirectory(ConfigsFolder);

            // 加载所有配置
            LoadAllConfigs();

            // 如果没有配置，创建默认配置
            if (_configs.Count == 0)
            {
                var defaultConfig = CreateDefaultConfig();
                _configs.Add(defaultConfig);
                SaveConfig(defaultConfig);
            }

            CurrentConfig = _configs.First();
        }

        private LaunchConfig CreateDefaultConfig()
        {
            var config = new LaunchConfig
            {
                Name = "默认配置",
                FilePath = GetConfigFilePath("默认配置")
            };
            config.ProcessItems.Add(new ProcessItem
            {
                Index = 1,
                ProcessPath = "3DMigoto",
                Arguments = "",
                Delay = 0
            });
            return config;
        }

        public string GetConfigFilePath(string configName)
        {
            return Path.Combine(ConfigsFolder, $"{configName}.json");
        }

        private void LoadAllConfigs()
        {
            _configs.Clear();

            if (!Directory.Exists(ConfigsFolder))
                return;

            var configFiles = Directory.GetFiles(ConfigsFolder, "*.json");
            foreach (var file in configFiles)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var config = JsonSerializer.Deserialize<LaunchConfig>(json);
                    config.FilePath = file;

                    // 重新设置序号
                    for (int i = 0; i < config.ProcessItems.Count; i++)
                    {
                        config.ProcessItems[i].Index = i + 1;
                    }

                    _configs.Add(config);
                }
                catch (Exception ex)
                {
                    // 处理配置文件损坏的情况
                    System.Diagnostics.Debug.WriteLine($"加载配置文件失败: {file}, 错误: {ex.Message}");
                }
            }
        }

        public void SaveConfig(LaunchConfig config)
        {
            try
            {
                config.LastModified = DateTime.Now;
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(config.FilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存配置文件失败: {ex.Message}");
            }
        }

        public void AddNewConfig(string name)
        {
            var newConfig = new LaunchConfig
            {
                Name = name,
                FilePath = GetConfigFilePath(name)
            };
            newConfig.ProcessItems.Add(new ProcessItem
            {
                Index = 1,
                ProcessPath = "3DMigoto",
                Arguments = "",
                Delay = 0
            });

            _configs.Add(newConfig);
            SaveConfig(newConfig);
            CurrentConfig = newConfig;
        }

        public void DeleteConfig(LaunchConfig config)
        {
            if (_configs.Count <= 1)
                return;

            try
            {
                if (File.Exists(config.FilePath))
                    File.Delete(config.FilePath);

                _configs.Remove(config);

                if (CurrentConfig == config)
                    CurrentConfig = _configs.First();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"删除配置文件失败: {ex.Message}");
            }
        }

        public void RefreshProcessListIndex(LaunchConfig config)
        {
            for (int i = 0; i < config.ProcessItems.Count; i++)
            {
                config.ProcessItems[i].Index = i + 1;
            }
        }

        public ObservableCollection<LaunchConfig> GetConfigs()
        {
            return _configs;
        }

        public async void ExecuteCurrentConfig()
        {
            if (CurrentConfig == null) return;

            foreach (var processItem in CurrentConfig.ProcessItems.OrderBy(item => item.Index))
            {
                if (!string.IsNullOrEmpty(processItem.ProcessPath) && File.Exists(processItem.ProcessPath))
                {
                    try
                    {
                        var process = Process.Start(processItem.ProcessPath, processItem.Arguments ?? "");

                        if (processItem.Delay > 0)
                        {
                            await Task.Delay(processItem.Delay);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"启动进程失败: {ex.Message}");
                    }
                }
            }
        }

        private bool _is3DmPlaceholder;
        public bool Is3DmPlaceholder
        {
            get => _is3DmPlaceholder;
            set { _is3DmPlaceholder = value; OnPropertyChanged(); }
        }

        private void OnPropertyChanged()
        {
            throw new NotImplementedException();
        }
    }

    public class ProcessPathToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string text = value?.ToString() ?? "";
            string lower = text.ToLower();

            bool contains3dm = lower.Contains("3dm");
            bool containsInvalid =
                lower.Contains(":") ||
                lower.Contains("：") ||
                lower.Contains("/") ||
                lower.Contains("／") ||
                lower.Contains("\\") ||
                lower.Contains("＼");

            // 文本中“有3dm” 且 “没有 冒号/斜杠”
            if (contains3dm && !containsInvalid)
            {
                // 吃掉右边按钮
                return Visibility.Collapsed;
            }

            // 否则正常显示按钮
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static object Inverse(object value, Type targetType, object parameter, string language)
        {
            var visibility = (Visibility)value;
            return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
    public class ArgumentsPlaceholderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string text = value?.ToString() ?? "";
            string lower = text.ToLower();

            // 含3dm
            bool contains3dm = lower.Contains("3dm");

            // 禁止出现冒号、斜杠（半角全角）
            bool containsInvalid =
                lower.Contains(":") ||
                lower.Contains("：") ||
                lower.Contains("/") ||
                lower.Contains("／") ||
                lower.Contains("\\") ||
                lower.Contains("＼");

            if (contains3dm && !containsInvalid)
            {
                return "注入路径";   // 3dm 模式
            }

            return "启动参数";        // 默认模式
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PathToInverseVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string text = value?.ToString() ?? "";
            bool is3dm = text.ToLower().Contains("3dm");

            bool containsInvalid =
                text.Contains(":") || text.Contains("：") ||
                text.Contains("/") || text.Contains("／") ||
                text.Contains("\\") || text.Contains("＼");

            bool match = is3dm && !containsInvalid;

            // 原：match → Collapsed / 非 match → Visible
            // 反转：match → Visible / 非 match → Collapsed
            return match ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}