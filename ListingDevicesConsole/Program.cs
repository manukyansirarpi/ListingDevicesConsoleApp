using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;
using Newtonsoft.Json;
using Accord.DirectSound;
using System.Xml;

namespace ListingDevicesConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Device> videoCaptureDevices = new List<Device>();
            List<StudioDevice> audioOutputs = new List<StudioDevice>();
            List<StudioDevice> audioInputs = new List<StudioDevice>();

            DsDevice[] videoDevices = DirectShowLib.DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            foreach (DsDevice dev in videoDevices)
            {
                string id = PrepareVideoId(dev);

                videoCaptureDevices.Add(new Device { category = "video", name = dev.Name, path = dev.DevicePath, id = id });
            }

            // Load audio devices and set the correct selected ones
            var inputDevices = new AudioDeviceCollection(AudioDeviceCategory.Capture);
            audioInputs = inputDevices
                //.Where(IsSelectableDevice)
                .Select(device =>
               new StudioDevice
               {
                   Id = $"{{0.0.1.00000000}}.{{{device.Guid}}}",
                   Name = device.Description
               }).ToList();

            var outputDevices = new AudioDeviceCollection(AudioDeviceCategory.Output);
            audioOutputs = outputDevices
                //.Where(IsSelectableDevice)
                .Select(device =>
                 new StudioDevice
                 {
                     Id = $"{{0.0.0.00000000}}.{{{device.Guid}}}",
                     Name = device.Description
                 }).ToList();

            generateJson(videoCaptureDevices, audioInputs, audioOutputs);
        }

        private static string PrepareVideoId(DsDevice device)
        {
            var index = device.DevicePath.IndexOf("\\", StringComparison.Ordinal);

            return device.DevicePath.IndexOf(":sw:", StringComparison.OrdinalIgnoreCase) > 0
                ? $"{device.Name}:"
                : $"{device.Name}:{device.DevicePath.Substring(index)}";
        }

        private static void generateJson(List<Device> videoCaptureDevices, List<StudioDevice> audioInputs, List<StudioDevice> audioOutputs)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "com.digitalclassroom.dev");

            string _videoCaptureDevices = JsonConvert.SerializeObject(videoCaptureDevices, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "videoDevices.json"), _videoCaptureDevices);


            string _audioOutputs = JsonConvert.SerializeObject(audioOutputs, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "audioOutputs.json"), _audioOutputs);

            string _audioInputs = JsonConvert.SerializeObject(audioInputs, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path.Combine(path, "audioInputs.json"), _audioInputs);
        }
    }
}
