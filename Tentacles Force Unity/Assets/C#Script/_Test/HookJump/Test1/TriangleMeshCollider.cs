using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TriangleMeshCollider : MonoBehaviour
{
    void Start()
    {
        // メッシュの作成
        Mesh mesh = new Mesh();

        // 頂点情報（三角形の3点）
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0), // 頂点1
            new Vector3(1, 0, 0), // 頂点2
            new Vector3(0, 1, 0)  // 頂点3
        };

        // 三角形の定義（頂点の順序）
        int[] triangles = new int[]
        {
            0, 1, 2  // 1 → 2 → 3 の順に描画
        };

        // メッシュに頂点と三角形情報を設定
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // 法線を自動計算（ライティング用）

        // MeshFilter にセット（視覚的に確認するため）
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // メッシュコライダーを設定
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true; // 物理エンジンと互換性を持たせる場合
    }
}