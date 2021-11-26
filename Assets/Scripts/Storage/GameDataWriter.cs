using System.IO;
using UnityEngine;

public class GameDataWriter {
    private BinaryWriter _writer;

    public GameDataWriter(BinaryWriter writer) {
        _writer = writer;
    }

    public void Write(int value) {
        _writer.Write(value);
    }

    public void Wite(float value) {
        _writer.Write(value);
    }

    public void Write(string value) {
        _writer.Write(value);
    }

    public void Write(Vector3 value) {
        _writer.Write(value.x);
        _writer.Write(value.y);
        _writer.Write(value.z);
    }

    public void Write(Vector3Int value) {
        _writer.Write(value.x);
        _writer.Write(value.y);
        _writer.Write(value.z);
    }
}
