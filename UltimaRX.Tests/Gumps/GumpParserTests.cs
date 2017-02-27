﻿using System;
using FluentAssertions;
using Infusion.Gumps;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Gumps
{
    [TestClass]
    public class GumpParserTests
    {
        private GumpParserDescriptionProcessor processor;
        private GumpParser parser;

        [TestInitialize]
        public void Initialize()
        {
            processor = new GumpParserDescriptionProcessor();
            parser = new GumpParser(processor);
        }

        [TestMethod]
        public void Can_parse_trigger_button()
        {
            var gump = new Gump(0, 1, "{Button 13 158 4005 4007 1 0 2}", new string[] { });

            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be($"Button: x = 13, y = 158, isTrigger, pageId = 0, triggerId = 2{Environment.NewLine}");
        }

        [TestMethod]
        public void Can_parse_nonTrigger_button()
        {
            var gump = new Gump(0, 1, "{Button 13 158 4005 4007 0 0 0}", new string[] { });

            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be($"Button: x = 13, y = 158, pageId = 0, triggerId = 0{Environment.NewLine}");
        }

        [TestMethod]
        public void Can_parse_two_buttons()
        {
            var gump = new Gump(0, 1, "{Button 13 158 4005 4007 1 0 2}{Button 13 158 4005 4007 0 0 0}", new string[] { });

            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be($"Button: x = 13, y = 158, isTrigger, pageId = 0, triggerId = 2{Environment.NewLine}Button: x = 13, y = 158, pageId = 0, triggerId = 0{Environment.NewLine}");
        }

        [TestMethod]
        public void Can_parse_text()
        {
            var gump = new Gump(0, 1, "{Text 164 13 955 0}", new[] { "This is a test text" });

            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be($"Text: x = 164, y = 13, hue = 955, This is a test text{Environment.NewLine}");
        }

        [TestMethod]
        public void Can_parse_unknown_control()
        {
            var gump = new Gump(0, 1, "{Unknown 159 13 88 19 2624}", new string[0]);

            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be(string.Empty);
        }

        [TestMethod]
        public void Can_parse_button_after_unknown_control()
        {
            var gump = new Gump(0, 1, "{Unknown 159 13 88 19 2624}{Button 13 158 4005 4007 0 0 0}", new string[0]);

            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be($"Button: x = 13, y = 158, pageId = 0, triggerId = 0{Environment.NewLine}");
        }

        [TestMethod]
        public void Can_parse_button_before_unknown_control()
        {
            var gump = new Gump(0, 1, "{Button 13 158 4005 4007 0 0 0}{Unknown 159 13 88 19 2624}", new string[0]);

            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be($"Button: x = 13, y = 158, pageId = 0, triggerId = 0{Environment.NewLine}");
        }

        [TestMethod]
        public void Can_parse_button_between_unknown_control()
        {
            var gump = new Gump(0, 1, "{Unknown 159 13 88 19 2624}{Button 13 158 4005 4007 0 0 0}{Unknown 159 13 88 19 2624}", new string[0]);

            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be($"Button: x = 13, y = 158, pageId = 0, triggerId = 0{Environment.NewLine}");
        }

        [TestMethod]
        public void Can_parse_unknown_control_between_buttons()
        {
            var gump = new Gump(0, 1, "{Button 13 158 4005 4007 0 0 0}{Unknown 159 13 88 19 2624}{Button 13 158 4005 4007 0 0 0}", new string[0]);

            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be($"Button: x = 13, y = 158, pageId = 0, triggerId = 0{Environment.NewLine}Button: x = 13, y = 158, pageId = 0, triggerId = 0{Environment.NewLine}");
        }

        [TestMethod]
        public void Can_parse_complex_gump()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0xB0, // packet
                0x05, 0x72, // size
                0x40, 0x00, 0x0D, 0xA7, // Id
                0x96, 0x00, 0x04, 0x95, // GumpId
                0x00, 0x00, 0x00, 0x1E, // X
                0x00, 0x00, 0x00, 0x00, // Y
                0x03, 0x1B, // Command Section Length (795)
                0x7B, 0x52, 0x65, 0x73, 0x69, 0x7A, 0x65, 0x50, 0x69, 0x63, 0x20, 0x30, 0x20, 0x30, 0x20, 0x39,
                0x32, 0x35, 0x30, 0x20, 0x32, 0x36, 0x30, 0x20, 0x32, 0x34, 0x36, 0x7D, 0x7B, 0x47, 0x75, 0x6D,
                0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x33, 0x20, 0x31, 0x33, 0x20,
                0x31, 0x34, 0x33, 0x20, 0x31, 0x39, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75, 0x6D,
                0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x35, 0x39, 0x20, 0x31, 0x33,
                0x20, 0x38, 0x38, 0x20, 0x31, 0x39, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75, 0x6D,
                0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x33, 0x20, 0x33, 0x35, 0x20,
                0x31, 0x34, 0x33, 0x20, 0x31, 0x39, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75, 0x6D,
                0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x35, 0x39, 0x20, 0x33, 0x35,
                0x20, 0x38, 0x38, 0x20, 0x31, 0x39, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75, 0x6D,
                0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x33, 0x20, 0x35, 0x37, 0x20,
                0x31, 0x34, 0x33, 0x20, 0x31, 0x39, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75, 0x6D,
                0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x35, 0x39, 0x20, 0x35, 0x37,
                0x20, 0x38, 0x38, 0x20, 0x31, 0x39, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75, 0x6D,
                0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x33, 0x20, 0x37, 0x39, 0x20,
                0x31, 0x34, 0x33, 0x20, 0x31, 0x39, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75, 0x6D,
                0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x35, 0x39, 0x20, 0x37, 0x39,
                0x20, 0x38, 0x38, 0x20, 0x31, 0x39, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75, 0x6D,
                0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x33, 0x20, 0x31, 0x31, 0x37,
                0x20, 0x32, 0x33, 0x34, 0x20, 0x33, 0x38, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75,
                0x6D, 0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x31, 0x33, 0x20, 0x31, 0x35,
                0x38, 0x20, 0x32, 0x39, 0x20, 0x37, 0x36, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x47, 0x75,
                0x6D, 0x70, 0x50, 0x69, 0x63, 0x54, 0x69, 0x6C, 0x65, 0x64, 0x20, 0x34, 0x35, 0x20, 0x31, 0x35,
                0x38, 0x20, 0x32, 0x30, 0x32, 0x20, 0x37, 0x36, 0x20, 0x32, 0x36, 0x32, 0x34, 0x7D, 0x7B, 0x43,
                0x68, 0x65, 0x63, 0x6B, 0x65, 0x72, 0x54, 0x72, 0x61, 0x6E, 0x73, 0x20, 0x31, 0x30, 0x20, 0x31,
                0x30, 0x20, 0x32, 0x34, 0x30, 0x20, 0x32, 0x32, 0x37, 0x7D, 0x7B, 0x47, 0x75, 0x6D, 0x70, 0x50,
                0x69, 0x63, 0x20, 0x31, 0x33, 0x20, 0x31, 0x33, 0x20, 0x31, 0x30, 0x30, 0x7D, 0x7B, 0x48, 0x54,
                0x4D, 0x4C, 0x47, 0x75, 0x6D, 0x70, 0x20, 0x33, 0x33, 0x20, 0x33, 0x33, 0x20, 0x31, 0x30, 0x30,
                0x20, 0x36, 0x30, 0x20, 0x31, 0x30, 0x30, 0x20, 0x30, 0x20, 0x30, 0x7D, 0x7B, 0x54, 0x65, 0x78,
                0x74, 0x20, 0x31, 0x36, 0x34, 0x20, 0x31, 0x33, 0x20, 0x39, 0x35, 0x35, 0x20, 0x31, 0x30, 0x31,
                0x7D, 0x7B, 0x54, 0x65, 0x78, 0x74, 0x20, 0x31, 0x36, 0x34, 0x20, 0x33, 0x35, 0x20, 0x39, 0x35,
                0x35, 0x20, 0x31, 0x30, 0x32, 0x7D, 0x7B, 0x54, 0x65, 0x78, 0x74, 0x20, 0x31, 0x36, 0x34, 0x20,
                0x35, 0x37, 0x20, 0x39, 0x35, 0x35, 0x20, 0x31, 0x30, 0x33, 0x7D, 0x7B, 0x54, 0x65, 0x78, 0x74,
                0x20, 0x31, 0x36, 0x34, 0x20, 0x37, 0x39, 0x20, 0x39, 0x35, 0x35, 0x20, 0x31, 0x30, 0x34, 0x7D,
                0x7B, 0x54, 0x65, 0x78, 0x74, 0x20, 0x31, 0x38, 0x20, 0x31, 0x31, 0x37, 0x20, 0x39, 0x35, 0x35,
                0x20, 0x31, 0x30, 0x35, 0x7D, 0x7B, 0x54, 0x65, 0x78, 0x74, 0x20, 0x31, 0x38, 0x20, 0x31, 0x33,
                0x36, 0x20, 0x39, 0x35, 0x35, 0x20, 0x31, 0x30, 0x36, 0x7D, 0x7B, 0x54, 0x65, 0x78, 0x74, 0x20,
                0x35, 0x30, 0x20, 0x31, 0x35, 0x38, 0x20, 0x39, 0x35, 0x35, 0x20, 0x31, 0x30, 0x37, 0x7D, 0x7B,
                0x42, 0x75, 0x74, 0x74, 0x6F, 0x6E, 0x20, 0x31, 0x33, 0x20, 0x31, 0x35, 0x38, 0x20, 0x34, 0x30,
                0x30, 0x35, 0x20, 0x34, 0x30, 0x30, 0x37, 0x20, 0x31, 0x20, 0x30, 0x20, 0x32, 0x7D, 0x7B, 0x54,
                0x65, 0x78, 0x74, 0x20, 0x35, 0x30, 0x20, 0x31, 0x37, 0x37, 0x20, 0x39, 0x35, 0x35, 0x20, 0x31,
                0x30, 0x38, 0x7D, 0x7B, 0x42, 0x75, 0x74, 0x74, 0x6F, 0x6E, 0x20, 0x31, 0x33, 0x20, 0x31, 0x37,
                0x37, 0x20, 0x34, 0x30, 0x30, 0x35, 0x20, 0x34, 0x30, 0x30, 0x37, 0x20, 0x31, 0x20, 0x30, 0x20,
                0x34, 0x7D, 0x7B, 0x54, 0x65, 0x78, 0x74, 0x20, 0x35, 0x30, 0x20, 0x31, 0x39, 0x36, 0x20, 0x39,
                0x35, 0x35, 0x20, 0x31, 0x30, 0x39, 0x7D, 0x7B, 0x42, 0x75, 0x74, 0x74, 0x6F, 0x6E, 0x20, 0x31,
                0x33, 0x20, 0x31, 0x39, 0x36, 0x20, 0x34, 0x30, 0x30, 0x35, 0x20, 0x34, 0x30, 0x30, 0x37, 0x20,
                0x31, 0x20, 0x30, 0x20, 0x36, 0x7D, 0x7B, 0x54, 0x65, 0x78, 0x74, 0x20, 0x35, 0x30, 0x20, 0x32,
                0x31, 0x35, 0x20, 0x39, 0x35, 0x35, 0x20, 0x31, 0x31, 0x30, 0x7D, 0x7B, 0x42, 0x75, 0x74, 0x74,
                0x6F, 0x6E, 0x20, 0x31, 0x33, 0x20, 0x32, 0x31, 0x35, 0x20, 0x34, 0x30, 0x30, 0x35, 0x20, 0x34,
                0x30, 0x30, 0x37, 0x20, 0x31, 0x20, 0x30, 0x20, 0x39, 0x7D, 0x00,
                0x00, 0x6F, // TextLinesCount = 111
                // 100 empty lines
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x20, // line 100 text length
                // line 100 unicode text
                0x00, 0x44, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x20, 0x00, 0x63, 0x00, 0x69, 0x00, 0x73, 0x00, 0x6C,
                0x00, 0x6F, 0x00, 0x20, 0x00, 0x31, 0x00, 0x38, 0x00, 0x38, 0x00, 0x3C, 0x00, 0x62, 0x00, 0x72,
                0x00, 0x3E, 0x00, 0x44, 0x00, 0x75, 0x00, 0x6D, 0x00, 0x20, 0x00, 0x70, 0x00, 0x72, 0x00, 0x61,
                0x00, 0x74, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x20, 0x00, 0x70, 0x00, 0x69, 0x00, 0x76, 0x00, 0x61,
                0x00, 0x08, // line 101 text length
                // line 101 unicode text
                0x00, 0x4D, 0x00, 0x61, 0x00, 0x6A, 0x00, 0x69, 0x00, 0x74, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x3A,
                0x00, 0x05, // line 102 text length
                // line 102 unicode text
                0x00, 0x4A, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6B, 0x00, 0x79,
                0x00, 0x0C, // line 103 text length
                // line 103 unicode text
                0x00, 0x54, 0x00, 0x76, 0x00, 0x75, 0x00, 0x6A, 0x00, 0x20, 0x00, 0x73, 0x00, 0x74, 0x00, 0x61,
                0x00, 0x74, 0x00, 0x75, 0x00, 0x73, 0x00, 0x3A,
                0x00, 0x06, // line 104 text length
                // line 104 unicode text
                0x00, 0x50, 0x00, 0x72, 0x00, 0x69, 0x00, 0x74, 0x00, 0x65, 0x00, 0x6C,
                0x00, 0x10, // line 105 text length
                // line 105 unicode text
                0x00, 0x4E, 0x00, 0x61, 0x00, 0x6A, 0x00, 0x65, 0x00, 0x6D, 0x00, 0x3A, 0x00, 0x20, 0x00, 0x31,
                0x00, 0x32, 0x00, 0x35, 0x00, 0x30, 0x00, 0x30, 0x00, 0x30, 0x00, 0x20, 0x00, 0x67, 0x00, 0x70,
                0x00, 0x13, // line 106 text length
                // line 106 unicode text
                0x00, 0x5A, 0x00, 0x61, 0x00, 0x70, 0x00, 0x6C, 0x00, 0x61, 0x00, 0x63, 0x00, 0x65, 0x00, 0x6E,
                0x00, 0x6F, 0x00, 0x20, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x20, 0x00, 0x36, 0x00, 0x35, 0x00, 0x20,
                0x00, 0x64, 0x00, 0x6E, 0x00, 0x69,
                0x00, 0x16, // line 107 text length
                // line 107 unicode text
                0x00, 0x5A, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x6B, 0x00, 0x6E, 0x00, 0x6F, 0x00, 0x75, 0x00, 0x74,
                0x00, 0x2F, 0x00, 0x6F, 0x00, 0x64, 0x00, 0x65, 0x00, 0x6D, 0x00, 0x6B, 0x00, 0x6E, 0x00, 0x6F,
                0x00, 0x75, 0x00, 0x74, 0x00, 0x20, 0x00, 0x76, 0x00, 0x65, 0x00, 0x63,
                0x00, 0x12, // line 108 text length
                // line 108 unicode text
                0x00, 0x53, 0x00, 0x65, 0x00, 0x7A, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x20, 0x00, 0x70,
                0x00, 0x72, 0x00, 0x61, 0x00, 0x74, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x20, 0x00, 0x64, 0x00, 0x6F,
                0x00, 0x6D, 0x00, 0x75,
                0x00, 0x1A, // line 109 text length
                // line 109 unicode text
                0x00, 0x53, 0x00, 0x65, 0x00, 0x7A, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x20, 0x00, 0x73,
                0x00, 0x70, 0x00, 0x6F, 0x00, 0x6C, 0x00, 0x75, 0x00, 0x76, 0x00, 0x6C, 0x00, 0x61, 0x00, 0x73,
                0x00, 0x74, 0x00, 0x6E, 0x00, 0x69, 0x00, 0x6B, 0x00, 0x75, 0x00, 0x20, 0x00, 0x64, 0x00, 0x6F,
                0x00, 0x6D, 0x00, 0x75,
                0x00, 0x0D, // line 110 text length
                // line 110 unicode text
                0x00, 0x4F, 0x00, 0x74, 0x00, 0x65, 0x00, 0x76, 0x00, 0x72, 0x00, 0x69, 0x00, 0x74, 0x00, 0x20,
                0x00, 0x62, 0x00, 0x61, 0x00, 0x6E, 0x00, 0x6B, 0x00, 0x75
            });

            var packet = new SendGumpMenuDialogPacket();
            packet.Deserialize(rawPacket);

            var gump = new Gump(packet.Id, packet.GumpId, packet.Commands, packet.TextLines);
            parser.Parse(gump);
            string description = processor.GetDescription();

            description.Should().Be(@"Text: x = 164, y = 13, hue = 955, Majitel:
Text: x = 164, y = 35, hue = 955, Jooky
Text: x = 164, y = 57, hue = 955, Tvuj status:
Text: x = 164, y = 79, hue = 955, Pritel
Text: x = 18, y = 117, hue = 955, Najem: 125000 gp
Text: x = 18, y = 136, hue = 955, Zaplaceno na 65 dni
Text: x = 50, y = 158, hue = 955, Zamknout/odemknout vec
Button: x = 13, y = 158, isTrigger, pageId = 0, triggerId = 2
Text: x = 50, y = 177, hue = 955, Seznam pratel domu
Button: x = 13, y = 177, isTrigger, pageId = 0, triggerId = 4
Text: x = 50, y = 196, hue = 955, Seznam spoluvlastniku domu
Button: x = 13, y = 196, isTrigger, pageId = 0, triggerId = 6
Text: x = 50, y = 215, hue = 955, Otevrit banku
Button: x = 13, y = 215, isTrigger, pageId = 0, triggerId = 9
");
        }
    }
}
