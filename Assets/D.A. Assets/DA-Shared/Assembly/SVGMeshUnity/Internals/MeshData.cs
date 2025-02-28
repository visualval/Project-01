﻿/*
MIT License

Copyright (c) 2018 Yoshihiro Shindo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.SVGMeshUnity.Internals
{
    public class MeshData
    {
        public MeshData()
        {
            Vertices = new List<Vector3>();
            Edges = new List<Int2>();
            Triangles = new List<int>();
            VertexIndices = new Hashtable();
        }
        
        public List<Vector3> Vertices { get; private set; }
        public List<Int2> Edges { get; private set; }
        public List<int> Triangles { get; private set; }

        private Hashtable VertexIndices;

        public void Clear()
        {
            Vertices.Clear();
            Edges.Clear();
            Triangles.Clear();
            VertexIndices.Clear();
        }

        public void AddVertices(WorkBuffer<Vector2> buffer)
        {
            var firstEdgeIdx = -1;
            var prevEdgeidx = -1;

            var vertices = Vertices;
            var edges = Edges;
            var indicies = VertexIndices;

            var size = buffer.UsedSize;
            var data = buffer.Data;

            for (var i = 0; i < size; ++i)
            {
                var v = data[i];
                var idx = -1;

                var index = indicies[v];
                if (index != null)
                {
                    idx = (int) index;
                }
                
                if (idx == -1)
                {
                    vertices.Add(v);
                    idx = vertices.Count - 1;
                    indicies[v] = idx;
                }

                if (idx == prevEdgeidx)
                {
                    continue;
                }
                
                if (i == 0)
                {
                    firstEdgeIdx = idx;
                }
                else
                {
                    edges.Add(new Int2(prevEdgeidx, idx));
                }

                prevEdgeidx = idx;
            }

            if (prevEdgeidx != firstEdgeIdx)
            {
                edges.Add(new Int2(prevEdgeidx, firstEdgeIdx));
            }
        }

        public void MakeUnityFriendly()
        {
            {
                var vertices = Vertices;
                var l = vertices.Count;
                for (var i = 0; i < l; ++i)
                {
                    var v = vertices[i];
                    v.y *= -1f;
                    vertices[i] = v;
                }
            }
            {
                var triangles = Triangles;
                var l = triangles.Count;
                for (var i = 0; i < l; i += 3)
                {
                    var a = triangles[i + 0];
                    var b = triangles[i + 1];
                    var c = triangles[i + 2];
                    triangles[i + 0] = b;
                    triangles[i + 1] = c;
                    triangles[i + 2] = a;
                }
            }
        }

        public void Upload(Mesh m)
        {
            m.Clear();
            m.SetVertices(Vertices);
            m.SetTriangles(Triangles, 0);
            m.RecalculateBounds();
            m.RecalculateNormals();
        }

        public void DumpTriangles()
        {
            Debug.Log(Triangles.Aggregate("", (_, i) => _ + i.ToString() + "\n"));
        }
    }
}