using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;

namespace atg.ImagingUWP.ViewModels
{
    public class MainViewModel
    {
        private PropertySet m_configurationPropertySet;
        public RelayCommand ToggleCameraCommand { get; set; }
        public RelayCommand<FrameworkElement> PreviewVideoElementLoadedCommand { get; set; }

        private Windows.Devices.Enumeration.Panel m_desiredCameraPanel;

        public MainViewModel()
        {
            PreviewVideoElementLoadedCommand = new RelayCommand<FrameworkElement>(OnFrameworkElementLoaded);
            m_configurationPropertySet = new PropertySet();

            Application.Current.Suspending += OnApplicationSuspending;
            Application.Current.Resuming += OnApplicationResuming;
            SystemMediaTransportControls.GetForCurrentView().PropertyChanged += OnMediaPropertyChanged;

            ToggleCameraCommand = new RelayCommand(OnToggleCamera, CanExecuteToggleCamera);
            
            m_desiredCameraPanel = Windows.Devices.Enumeration.Panel.Back;

            UpdateConfiguration();

            CurrentState = "Normal";
            CurrentEditor = Effects.FirstOrDefault();

            IsRecordingEnabled = true;
            m_externalCamera = false;
            m_isToggelingCamera = false;
        }
        private async void OnToggleCamera()
        {
            m_isToggelingCamera = true;
            UpdateButtonState();
            await StopPreviewAsync();
            if (m_desiredCameraPanel == Windows.Devices.Enumeration.Panel.Back)
            {
                m_desiredCameraPanel = Windows.Devices.Enumeration.Panel.Front;
            }
            else
            {
                m_desiredCameraPanel = Windows.Devices.Enumeration.Panel.Back;
            }
            await StartPreviewAsync();
            m_isToggelingCamera = false;
            UpdateButtonState();
        }

        private async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            m_hasFrontAndBackCamera = allVideoDevices.Any(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front) &&
                allVideoDevices.Any(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, use the first device found
            desiredDevice = desiredDevice ?? allVideoDevices.FirstOrDefault();
            if (desiredDevice != null)
            {
                m_desiredCameraPanel = desiredPanel;
                m_mirroringPreview = desiredPanel == Windows.Devices.Enumeration.Panel.Front;
                if (desiredDevice.EnclosureLocation == null || desiredDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                {
                    // No information on the location of the camera, assume it's an external camera, not integrated on the device
                    m_externalCamera = true;
                }
                else
                {
                    m_externalCamera = false;
                }
            }

            return desiredDevice;
        }

    }
}
