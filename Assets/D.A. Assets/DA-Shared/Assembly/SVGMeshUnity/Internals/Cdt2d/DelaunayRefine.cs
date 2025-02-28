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


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.SVGMeshUnity.Internals.Cdt2d
{
    public class DelaunayRefine
    {
        public WorkBufferPool WorkBufferPool;
        
        //Assume edges are sorted lexicographically
        public void RefineTriangles(Triangles triangles)
        {
            var stack = WorkBufferPool.Get<Int2>();

            var points = triangles.Vertices;
            var numPoints = points.Count;
            var stars = triangles.Stars;
            for (var a = 0; a < numPoints; ++a)
            {
                var star = stars[a];
                var starData = star.Data;
                var sl = star.UsedSize;
                for (var j = 0; j < sl; ++j)
                {
                    var s = starData[j];
                    var b = s.y;

                    //If order is not consistent, then skip edge
                    if (b < a)
                    {
                        continue;
                    }

                    //Check if edge is constrained
                    if (triangles.IsConstraint(a, b))
                    {
                        continue;
                    }

                    //Find opposite edge
                    var x = s.x;
                    var y = -1;
                    for (var k = 0; k < sl; ++k)
                    {
                        if (starData[k].x == b)
                        {
                            y = starData[k].y;
                            break;
                        }
                    }

                    //If this is a boundary edge, don't flip it
                    if (y < 0)
                    {
                        continue;
                    }

                    //If edge is in circle, flip it
                    if (Robust.InSphere(points[a], points[b], points[x], points[y]) < 0f)
                    {
                        var v = new Int2(a, b);
                        stack.Push(ref v);
                    }
                }
            }

            while (stack.UsedSize > 0)
            {
                var v = stack.Pop();
                var a = v.x;
                var b = v.y;

                //Find opposite pairs
                var x = -1;
                var y = -1;
                var star = stars[a];
                var starData = star.Data;
                var sl = star.UsedSize;
                for (var i = 0; i < sl; ++i)
                {
                    var s = starData[i].x;
                    var t = starData[i].y;
                    if (s == b)
                    {
                        y = t;
                    }
                    else if (t == b)
                    {
                        x = s;
                    }
                }

                //If x/y are both valid then skip edge
                if (x < 0 || y < 0)
                {
                    continue;
                }

                //If edge is now delaunay, then don't flip it
                if (Robust.InSphere(points[a], points[b], points[x], points[y]) >= 0f)
                {
                    continue;
                }

                //Flip the edge
                triangles.Flip(a, b);

                //Test flipping neighboring edges
                TestFlip(points, triangles, stack, x, a, y);
                TestFlip(points, triangles, stack, a, y, x);
                TestFlip(points, triangles, stack, y, b, x);
                TestFlip(points, triangles, stack, b, x, y);
            }

            WorkBufferPool.Release(ref stack);
        }
        
        private void TestFlip(List<Vector3> points, Triangles triangles, WorkBuffer<Int2> stack, int a, int b, int x)
        {
            var y = triangles.Opposite(a, b);

            //Test boundary edge
            if (y < 0)
            {
                return;
            }

            //Swap edge if order flipped
            if (b < a)
            {
                var tmp = a;
                a = b;
                b = tmp;
                tmp = x;
                x = y;
                y = tmp;
            }

            //Test if edge is constrained
            if (triangles.IsConstraint(a, b))
            {
                return;
            }

            //Test if edge is delaunay
            if (Robust.InSphere(points[a], points[b], points[x], points[y]) < 0f)
            {
                var v = new Int2(a, b);
                stack.Push(ref v);
            }
        }
    }
}