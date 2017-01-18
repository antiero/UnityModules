﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DrawCurvedMesh : MonoBehaviour {

  public Behaviour toggler;

  void OnEnable() {
    toggler.enabled = false;
    foreach (var r in GetComponentsInChildren<Renderer>()) {
      r.enabled = false;
    }
  }

  void OnDisable() {
    foreach (var r in GetComponentsInChildren<Renderer>()) {
      r.enabled = true;
    }
    toggler.enabled = true;
  }

  public float radius = 1;
  void Update() {
    RadialPos p;
    p.radius = radius;
    p.angle = 0;
    p.height = 0;
    recurse(transform, p);
  }

  private void recurse(Transform t, RadialPos pos) {
    var r = t.GetComponent<Renderer>();
    if (r != null) {
      Mesh m = Instantiate(r.GetComponent<MeshFilter>().sharedMesh);
      m.hideFlags = HideFlags.HideAndDontSave;

      var verts = m.vertices;
      for (int i = 0; i < verts.Length; i++) {
        Vector3 vert = verts[i];
        Vector3 delta = t.TransformPoint(vert) - t.position;

        RadialPos vertPos;
        vertPos.radius = pos.radius + delta.z;
        vertPos.height = pos.height + delta.y;
        vertPos.angle = pos.angle + delta.x / pos.radius;

        Vector3 v;
        v.x = Mathf.Sin(vertPos.angle) * vertPos.radius;
        v.y = vertPos.height;
        v.z = Mathf.Cos(vertPos.angle) * vertPos.radius - radius;

        verts[i] = v;
      }

      m.vertices = verts;
      m.bounds = new Bounds(Vector3.zero, Vector3.one * 100000);
      m.RecalculateNormals();

      Graphics.DrawMesh(m, transform.localToWorldMatrix, r.sharedMaterial, 0);
    }

    foreach (Transform child in t) {
      Vector3 delta = child.position - t.position;

      RadialPos childPos;
      childPos.radius = pos.radius + delta.z;
      childPos.height = pos.height + delta.y;
      childPos.angle = pos.angle + delta.x / pos.radius;

      recurse(child, childPos);
    }
  }

  public struct RadialPos {
    public float radius;
    public float angle;
    public float height;
  }
}
