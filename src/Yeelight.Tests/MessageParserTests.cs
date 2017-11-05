using System;
using NUnit.Framework;

namespace Thoemmi.Yeelight.Tests {
    [TestFixture]
    public class MessageParserTests {
        [Test]
        public void ParseResponse() {
            var message = @"HTTP/1.1 200 OK
Cache-Control: max-age=3600
Date:
Ext:
Location: yeelight://192.168.1.239:55443
Server: POSIX UPnP/1.0 YGLC/1
id: 0x000000000015243f
model: color
fw_ver: 18
support: get_prop set_default set_power toggle set_bright start_cf stop_cf set_scene cron_add cron_get cron_del set_ct_abx set_rgb
power: on
bright: 100
color_mode: 2
ct: 4000
rgb: 16711680
hue: 100
sat: 35
name: my_bulb";

            var (reason, device) = MessageParser.Parse(message);

            Assert.AreEqual(Reason.Discovery, reason);
            Assert.AreEqual(TimeSpan.FromHours(1), device.RefreshInterval);
            Assert.AreEqual("192.168.1.239", device.EndPoint.Host);
            Assert.AreEqual(55443, device.EndPoint.Port);
            Assert.AreEqual("0x000000000015243f", device.Id);
            Assert.AreEqual("color", device.Model);
            Assert.AreEqual("18", device.FirmwareVersion);
            Assert.AreEqual(true, device.PoweredOn);
            Assert.AreEqual(100, device.Brightness);
            Assert.AreEqual(LightMode.ColorTemperature, device.LightMode);
            Assert.AreEqual(4000, device.ColorTemperature);
            Assert.AreEqual(0xff0000, device.RGB);
            Assert.AreEqual(100, device.Hue);
            Assert.AreEqual(35, device.Saturation);
            Assert.AreEqual("my_bulb", device.Name);
        }

        [Test]
        public void ParseAdvertisement() {
            var message = @"NOTIFY * HTTP/1.1
Host: 239.255.255.250:1982
Cache-Control: max-age=3600
Location: yeelight://192.168.1.239:55443
NTS: ssdp:alive
Server: POSIX, UPnP/1.0 YGLC/1
id: 0x000000000015243f
model: color
fw_ver: 18
support: get_prop set_default set_power toggle set_bright start_cf stop_cf set_scene cron_add cron_get cron_del set_ct_abx set_rgb
power: on
bright: 100
color_mode: 2
ct: 4000
rgb: 16711680
hue: 100
sat: 35
name: my_bulb";

            var (reason, device) = MessageParser.Parse(message);

            Assert.AreEqual(Reason.Advertisement, reason);
            Assert.AreEqual(TimeSpan.FromHours(1), device.RefreshInterval);
            Assert.AreEqual("192.168.1.239", device.EndPoint.Host);
            Assert.AreEqual(55443, device.EndPoint.Port);
            Assert.AreEqual("0x000000000015243f", device.Id);
            Assert.AreEqual("color", device.Model);
            Assert.AreEqual("18", device.FirmwareVersion);
            Assert.AreEqual(true, device.PoweredOn);
            Assert.AreEqual(100, device.Brightness);
            Assert.AreEqual(LightMode.ColorTemperature, device.LightMode);
            Assert.AreEqual(4000, device.ColorTemperature);
            Assert.AreEqual(0xff0000, device.RGB);
            Assert.AreEqual(100, device.Hue);
            Assert.AreEqual(35, device.Saturation);
            Assert.AreEqual("my_bulb", device.Name);
        }
    }
}