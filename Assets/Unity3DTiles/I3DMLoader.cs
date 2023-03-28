/*
 * Copyright 2018, by the California Institute of Technology. ALL RIGHTS 
 * RESERVED. United States Government Sponsorship acknowledged. Any 
 * commercial use must be negotiated with the Office of Technology 
 * Transfer at the California Institute of Technology.
 * 
 * This software may be subject to U.S.export control laws.By accepting 
 * this software, the user agrees to comply with all applicable 
 * U.S.export laws and regulations. User has the responsibility to 
 * obtain export licenses, or other export authority as may be required 
 * before exporting such information to foreign countries or providing 
 * access to foreign persons.
 */
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGLTF.Loader;

public class I3DMLoader : ILoader
{
    private ILoader loader;

    public I3DMLoader(ILoader loader)
    {
        this.loader = loader;

    }

    public Stream LoadedStream
    {
        get
        {
            return loader.LoadedStream;
        }
    }

    class FeatureTable
    {
#pragma warning disable 0649
        public int INSTANCES_LENGTH = 0;
        public ByteOffset POSITION = null;
        public ByteOffset POSITION_QUANTIZED = null;
        public ByteOffset QUANTIZED_VOLUME_OFFSET = null;
        public ByteOffset QUANTIZED_VOLUME_SCALE = null;
        public ByteOffset NORMAL_UP = null;
        public ByteOffset NORMAL_RIGHT = null;
        public ByteOffset NORMAL_UP_OCT32P = null;
        public ByteOffset NORMAL_RIGHT_OCT32P = null;
        public ByteOffset SCALE_NON_UNIFORM = null;
        public ByteOffset SCALE = null;
        public ByteOffset BATCH_ID = null;
        public bool EAST_NORTH_UP = true;
        public ByteOffset RTC_CENTER = null;
#pragma warning restore 0649
    }

    public IEnumerator LoadStream(string relativeFilePath)
    {
        yield return loader.LoadStream(relativeFilePath);

        if (loader.LoadedStream.Length > 0)
        {
            string filename = relativeFilePath.Split('?')[0]; // Remove query parameters if there are any

            if (Path.GetExtension(filename).ToLower() == ".i3dm")
            {
                BinaryReader br = new BinaryReader(loader.LoadedStream);
                UInt32 magic = br.ReadUInt32();
                if (magic != 0x6D643369)
                {
                    Debug.LogError("Unsupported magic number in i3dm: " + magic + " " + relativeFilePath);
                }
                UInt32 version = br.ReadUInt32();
                if (version != 1)
                {
                    Debug.LogError("Unsupported version number in i3dm: " + version + " " + relativeFilePath);
                }
                br.ReadUInt32(); // Total length
                UInt32 featureTableJsonLength = br.ReadUInt32();
                if (featureTableJsonLength == 0)
                {
                    Debug.LogError("Unexpected zero length JSON feature table in i3dm: " + relativeFilePath);
                }
                UInt32 featureTableBinaryLength = br.ReadUInt32();
                if (featureTableBinaryLength != 0)
                {
                    Debug.LogError("Unexpected non-zero length binary feature table in i3dm: " + relativeFilePath);
                }
                UInt32 batchTableJsonLength = br.ReadUInt32();
                if (batchTableJsonLength != 0)
                {
                    Debug.LogError("Unexpected non-zero length JSON batch table in i3dm: " + relativeFilePath);
                }
                UInt32 batchTableBinaryLength = br.ReadUInt32();
                if (batchTableBinaryLength != 0)
                {
                    Debug.LogError("Unexpected non-zero length binary batch table in i3dm: " + relativeFilePath);
                }
                UInt32 gltfFormat = br.ReadUInt32();
                if (gltfFormat != 0 && gltfFormat != 1)
                {
                    Debug.LogError("Unexpected gltfFormat in i3dm: " + relativeFilePath);
                }
                string featureTableJson = new String(br.ReadChars((int)featureTableJsonLength));
                FeatureTable ft = JsonConvert.DeserializeObject<FeatureTable>(featureTableJson);
                if (ft.INSTANCES_LENGTH != 0)
                {
                    Debug.LogError("Unexpected non-zero length feature table BATCH_LENGTH in i3dm: " +
                                   relativeFilePath);
                }
                byte[] featureTableBinary = br.ReadBytes((int)featureTableBinaryLength);

            }
        }
    }
}
