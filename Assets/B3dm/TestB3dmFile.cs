/****************************************************
    文件：TestB3dmFile.cs
    作者：#CREATEAUTHOR#
    邮箱:  gaocanjun@baidu.com
    日期：#CREATETIME#
    功能：Todo
*****************************************************/
using B3dmCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class TestB3dmFile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //from glb to b3dm
    void Convert()
    {
        Debug.Log("Sample conversion from glb to b3dm.");
        var inputfile = @"testfixtures/building.glb";
        var buildingGlb = File.ReadAllBytes(inputfile);
        var b3dm = new B3dmCore.B3dm(buildingGlb);
        var bytes = b3dm.ToBytes();
        File.WriteAllBytes("test.b3dm", bytes);
        Debug.Log($"File building.b3dm is written...");
        Debug.Log($"Press any key to continue...");
    }

    //b3dm to glb and glTF/bin
    void ConvertToGlb()
    {
        Debug.Log("Sample code for unpacking a b3dm to glb and glTF/bin file");
        var inputfile = @"testfixtures/662.b3dm";
        var outputfile = Path.GetFileNameWithoutExtension(inputfile) + ".glb";
        var f = File.OpenRead(inputfile);
        var b3dm = B3dmReader.ReadB3dm(f);
        Debug.Log("b3dm version: " + b3dm.B3dmHeader.Version);
        var stream = new MemoryStream(b3dm.GlbData);
        //var gltf = SharpGLTF.Schema2.ModelRoot.ReadGLB(stream, new ReadSettings());
        //Debug.Log("glTF asset generator: " + gltf.Asset.Generator);
        //Debug.Log("glTF version: " + gltf.Asset.Version);
        //gltf.SaveGLB(outputfile);
        //Debug.Log("press any key to continue...");
    }

    //B3dmHeader
    public void HeaderToBinaryTests()
    {
        var b3dmHeader = new B3dmHeader
        {
            ByteLength = 2952
        };
        var binary = b3dmHeader.AsBinary();

        var errors = b3dmHeader.Validate();
        Debug.Log(binary.Length == 28);
    }

    Stream b3dmfile;
    string expectedMagicHeader = "b3dm";
    int expectedVersionHeader = 1;

    public void Setup()
    {
        b3dmfile = File.OpenRead(@"testfixtures/1_expected.b3dm");
        Debug.Log(b3dmfile != null);
    }

    //public void ReadB3dmWithPaddingTest()
    //{
    //    // arrange
    //    var b3dmfile = File.OpenRead(@"testfixtures/2_0_1.b3dm");

    //    // act
    //    var b3dm = B3dmReader.ReadB3dm(b3dmfile);
    //    var stream = new MemoryStream(b3dm.GlbData);
    //    var glb = SharpGLTF.Schema2.ModelRoot.ReadGLB(stream);
    //    Debug.Log(glb.Asset.Version.Major == 2.0);

    //    // assert
    //    Debug.Log(expectedMagicHeader == b3dm.B3dmHeader.Magic);
    //    Debug.Log(expectedVersionHeader == b3dm.B3dmHeader.Version);
    //    Debug.Log(b3dm.BatchTableJson.Length >= 0);
    //    Debug.Log(b3dm.GlbData.Length > 0);
    //}


    //public void ReadB3dmTest()
    //{
    //    // arrange

    //    // act
    //    var b3dm = B3dmReader.ReadB3dm(b3dmfile);
    //    var stream = new MemoryStream(b3dm.GlbData);
    //    var glb = SharpGLTF.Schema2.ModelRoot.ReadGLB(stream);
    //    Debug.Log(glb.Asset.Version.Major == 2.0);
    //    Debug.Log(glb.Asset.Generator == "SharpGLTF 1.0.0-alpha0009");

    //    // assert
    //    Debug.Log(expectedMagicHeader == b3dm.B3dmHeader.Magic);
    //    Debug.Log(expectedVersionHeader == b3dm.B3dmHeader.Version);
    //    Debug.Log(b3dm.BatchTableJson.Length >= 0);
    //    Debug.Log(b3dm.GlbData.Length > 0);
    //}


    public void ReadB3dmWithGlbTest()
    {
        // arrange
        var buildingGlb = File.ReadAllBytes(@"testfixtures/building.glb");

        // act

        var b3dm = new B3dmCore.B3dm(buildingGlb);

        // assert
        Debug.Log(b3dm.GlbData.Length == 2924);
    }

    public void ReadB3dmWithBatchTest()
    {
        // arrange
        var batchB3dm = File.OpenRead(@"testfixtures/with_batch.b3dm");
        var expectedBatchTableJsonText = File.ReadAllText(@"testfixtures/BatchTableJsonExpected.json");
        var expectedBatchTableJson = JObject.Parse(expectedBatchTableJsonText);

        // act
        var b3dm = B3dmReader.ReadB3dm(batchB3dm);
        var actualBatchTableJson = JObject.Parse(b3dm.BatchTableJson);

        // assert
        Debug.Log(b3dm.FeatureTableJson == "{\"BATCH_LENGTH\":12} ");
        Debug.Log(expectedBatchTableJson== actualBatchTableJson);
    }

    public void ReadNederland3DB3dmTest()
    {
        // arrange
        var b3dmfile1 = File.OpenRead(@"testfixtures/nederland3d_6825.b3dm");

        // act
        var b3dm = B3dmReader.ReadB3dm(b3dmfile1);

        // assert
        Debug.Log(expectedMagicHeader == b3dm.B3dmHeader.Magic);
        Debug.Log(expectedVersionHeader == b3dm.B3dmHeader.Version);
    }

    public void WriteB3dmWithCyrlllicCharacters()
    {
        // arrange
        var buildingGlb = File.ReadAllBytes(@"testfixtures/1.glb");
        var b3dm = new B3dmCore.B3dm(buildingGlb);
        var batchTableJson = File.ReadAllText(@"testfixtures/BatchTableWithCyrillicCharacters.json");
        b3dm.BatchTableJson = batchTableJson;
        b3dm.FeatureTableJson = "{\"BATCH_LENGTH\":12} ";

        // act
        var bytes = b3dm.ToBytes();
        var b3dmActual = B3dmReader.ReadB3dm(new MemoryStream(bytes));

        // Assert
        Debug.Log(b3dmActual.B3dmHeader.Validate().Count == 0);
    }

    public void WriteB3dmWithBatchTest()
    {
        // arrange
        var buildingGlb = File.ReadAllBytes(@"testfixtures/with_batch.glb");
        var batchTableJson = File.ReadAllText(@"testfixtures/BatchTableJsonExpected.json");

        var b3dmBytesExpected = File.OpenRead(@"testfixtures/with_batch.b3dm");
        var b3dmExpected = B3dmReader.ReadB3dm(b3dmBytesExpected);
        var errors = b3dmExpected.B3dmHeader.Validate();
        Debug.Log(errors.Count > 0);

        var b3dm = new B3dmCore.B3dm(buildingGlb);
        b3dm.FeatureTableJson = b3dmExpected.FeatureTableJson;
        b3dm.BatchTableJson = b3dmExpected.BatchTableJson;
        b3dm.FeatureTableBinary = b3dmExpected.FeatureTableBinary;
        b3dm.BatchTableBinary = b3dmExpected.BatchTableBinary;

        // act
        var result = "with_batch.b3dm";
        var bytes = b3dm.ToBytes();

        File.WriteAllBytes(result, bytes);

        var b3dmActual = B3dmReader.ReadB3dm(File.OpenRead(result));

        // Assert
        var errorsActual = b3dmActual.B3dmHeader.Validate();
        Debug.Log(errorsActual.Count == 0);

        Debug.Log(b3dmActual.B3dmHeader.Magic == b3dmExpected.B3dmHeader.Magic);
        Debug.Log(b3dmActual.B3dmHeader.Version == b3dmExpected.B3dmHeader.Version);
        Debug.Log(b3dmActual.B3dmHeader.FeatureTableJsonByteLength == b3dmExpected.B3dmHeader.FeatureTableJsonByteLength);
    }

    public void Initial()
    {
        var featureTableJson = "{\"INSTANCES_LENGTH\":2,\"POSITION\":{\"byteOffset\":0},\"EAST_NORTH_UP\":false,\"RTC_CENTER\":{\"byteOffset\":24}}";
        var paddedJson = BufferPadding.AddPadding(featureTableJson, 2);
        Debug.Log(paddedJson == "{\"INSTANCES_LENGTH\":2,\"POSITION\":{\"byteOffset\":0},\"EAST_NORTH_UP\":false,\"RTC_CENTER\":{\"byteOffset\":24}}       ");
    }
    public static bool IsSimilar(double first, double second)
    {
        var delta = 0.1;
        return (second > first - delta) && (second < first + delta);
    }
}
