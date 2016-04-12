using UnityEngine;
using System.Collections;
using UnityEditor;

public class AutoCityGenerator {
	[MenuItem("Assets/CheckStart")]
	static void BakeStart()
	{
		AutoCityGen(100);
	}

	static float startTime;
	static int nowResolution;

	static void TimeDump()
	{
		//Debug.Log ("Time:" + System.DateTime.Now.ToString());
		Debug.Log ("Reso\t" + nowResolution + "\tTime\t" + (Time.realtimeSinceStartup - startTime));
		if ( nowResolution < 1000 )
		{
			nowResolution += 100;
			AutoCityGen(nowResolution);
		}
	}

	static void AutoCityGen(int sizeX)
	{
		nowResolution = sizeX;
		GameObject root = GameObject.Find ("Buildings");
		if ( root ) {
			GameObject.DestroyImmediate(root);
			root = null;
		}

		//Light dirLight = GameObject.FindObjectOfType<Light>();
		GameObject go = GameObject.Find("Terrain");
		Terrain terrain = go.GetComponent<Terrain>();
		terrain.terrainData.size = new Vector3(sizeX, 1024.0f, sizeX);

		GameObject building = GameObject.Find ("Building");
		root = new GameObject();
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

		Lightmapping.completed = null;
		Lightmapping.completed = TimeDump;

//		Lightmapping.ClearDiskCache();
//		Lightmapping.Clear();
		Caching.CleanCache ();
		startTime = Time.realtimeSinceStartup;
		Lightmapping.BakeAsync();
	}

}
