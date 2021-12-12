using System.IO;
using UnityEngine;

public class GameDataReader {

	BinaryReader _reader;

	public GameDataReader(BinaryReader reader) {
		_reader = reader;
	}

	public float ReadFloat() {
		return _reader.ReadSingle();
	}

	public int ReadInt() {
		return _reader.ReadInt32();
	}

	public string ReadString() {
		return _reader.ReadString();
    }

	public Vector3 ReadVector3() {
		Vector3 value = new Vector3();
		value.x = _reader.ReadSingle();
		value.y = _reader.ReadSingle();
		value.z = _reader.ReadSingle();
		return value;
	}

	public Vector3Int ReadVector3Int() {
		Vector3Int value = new Vector3Int();
		value.x = _reader.ReadInt32();
		value.y = _reader.ReadInt32();
		value.z = _reader.ReadInt32();
		return value;
	}
}