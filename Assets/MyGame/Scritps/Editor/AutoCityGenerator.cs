using UnityEngine;
using System.Collections;
using UnityEditor;

public class AutoCityGenerator {
	[MenuItem("Assets/CheckStart")]
	static void BakeStart()
	{
		//AutoCityGen(100);
		nowSize = 200;
		nowDivide = 1;
		AutoCityGenWithMesh();
	}

	static float startTime;
	static int nowSize;
	static int nowDivide;

	static void NextAutoCityGenWithTerrain()
	{
		//Debug.Log ("Time:" + System.DateTime.Now.ToString());
		Debug.Log ("Reso\t" + nowSize + "\tTime\t" + (Time.realtimeSinceStartup - startTime));
		if ( nowSize < 1000 )
		{
			nowSize += 100;
			AutoCityGenWithTerrain();
		}
	}

	static void NextAutoCityGenWithMesh()
	{
		//Debug.Log ("Time:" + System.DateTime.Now.ToString());
		Debug.Log ("Reso\t" + nowSize + "\tTime\t" + (Time.realtimeSinceStartup - startTime));
		if ( nowSize < 1000 )
		{
			nowSize += 100;
			AutoCityGenWithMesh();
		}
	}

	static void Refresh()
	{
		GameObject root = GameObject.Find ("Buildings");
		if ( root ) {
			GameObject.DestroyImmediate(root);
			root = null;
		}
		GameObject root2 = GameObject.Find ("Grounds");
		if ( root2 ) {
			GameObject.DestroyImmediate(root2);
			root2 = null;
		}
	}

	static void AutoCityGenWithMesh()
	{
		Refresh();

		//Light dirLight = GameObject.FindObjectOfType<Light>();
		GameObject ground = GameObject.Find("Ground");
		GameObject grounds = new GameObject();
		grounds.name = "Grounds";
		int reso = nowDivide; // 4
		int step = nowSize / reso; // 25
		for( int ix=0 ; ix<nowDivide ; ix++ ) {
			for( int iy=0 ; iy<nowDivide ; iy++ ) {
				GameObject gd = GameObject.Instantiate(ground, new Vector3(ix*step, 0.0f, iy*step), Quaternion.identity) as GameObject;
				gd.transform.localScale = new Vector3(step*0.1f, 1.0f, step*0.1f);
				gd.transform.parent = grounds.transform;
				gd.isStatic = true;
				foreach( Transform tr in gd.GetComponentsInChildren<Transform>() ) {
					tr.gameObject.isStatic = true;
				}
			}
		}

		Lightmapping.completed = null;
		Lightmapping.completed = NextAutoCityGenWithMesh;

		MakeBuildings(nowSize);
		BakeLightmap();
	}

	static void AutoCityGenWithTerrain()
	{
		Refresh();

		//Light dirLight = GameObject.FindObjectOfType<Light>();
		GameObject go = GameObject.Find("Terrain");
		Terrain terrain = go.GetComponent<Terrain>();
		terrain.terrainData.size = new Vector3(nowSize, 1024.0f, nowSize);

		Lightmapping.completed = null;
		Lightmapping.completed = NextAutoCityGenWithTerrain;

		MakeBuildings(nowSize);
		BakeLightmap();
	}

	static void MakeBuildings(int sizeX)
	{
		GameObject building = GameObject.Find ("Building");
		GameObject root = new GameObject();
		root.name = "Buildings";

		int reso = 20;
		int step = sizeX / reso;
		for( int ix=0 ; ix<step ; ix++ ) {
			for( int iy=0 ; iy<step ; iy++ ) {
				GameObject bd = GameObject.Instantiate(building, new Vector3(ix*reso, 0.0f, iy*reso), Quaternion.identity) as GameObject;
				bd.transform.localScale = new Vector3(1.0f, Random.Range(1.0f, 5.0f), 1.0f);
				bd.transform.parent = root.transform;
				bd.isStatic = true;
				foreach( Transform tr in bd.GetComponentsInChildren<Transform>() ) {
					tr.gameObject.isStatic = true;
				}
			}
		}
	}

	static void BakeLightmap()
	{
		//		Lightmapping.ClearDiskCache();
		//		Lightmapping.Clear();
		Caching.CleanCache ();
		startTime = Time.realtimeSinceStartup;
		Lightmapping.BakeAsync();
	}

}
