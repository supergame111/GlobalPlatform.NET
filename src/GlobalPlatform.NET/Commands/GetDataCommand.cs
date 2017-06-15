﻿using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    public enum DataObject : ushort
    {
        IssuerIdentificationNumber = 0x0042,
        CardImageNumber = 0x0045,
        CardData = 0x0066,
        KeyInformationTemplate = 0x00E0,
        CardCapabilityInformation = 0x0067,
        CurrentSecurityLevel = 0x00D3,
        ListApplications = 0x2F00,
        ExtendedCardResourcesInformation = 0xFF21,
        SecurityDomainManagerUrl = 0x5F50
    }

    public interface IGetDataObjectPicker
    {
        IGetDataTagListPicker GetDataFrom(DataObject getDataObject);
    }

    public interface IGetDataTagListPicker : IApduBuilder
    {
        IApduBuilder WithTagList(params byte[] tags);
    }

    public class GetDataCommand : CommandBase<GetDataCommand, IGetDataObjectPicker>,
        IGetDataObjectPicker,
        IGetDataTagListPicker
    {
        private DataObject getDataObject;
        private byte[] tagList = new byte[0];

        public IGetDataTagListPicker GetDataFrom(DataObject getDataObject)
        {
            this.getDataObject = getDataObject;

            return this;
        }

        public IApduBuilder WithTagList(params byte[] tagList)
        {
            this.tagList = tagList;

            return this;
        }

        public override IEnumerable<Apdu> AsApdu()
        {
            var bytes = BitConverter.GetBytes((ushort)this.getDataObject);

            if (BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }

            byte p1 = bytes.First();
            byte p2 = bytes.Last();

            yield return Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.GetData, p1, p2, this.tagList);
        }
    }
}
