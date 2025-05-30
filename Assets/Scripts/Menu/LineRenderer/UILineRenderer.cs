using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic                              //check out GridRenderer script and https://www.youtube.com/watch?v=--LB7URk60A first
{
    public Vector2Int gridSize;
    public UIGridRenderer grid;

    public int pointCount;
    public List<Vector2> points;

    float width;
    float height;
    float unitWidth;
    float unitHeight;

    public float thickness = 10f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        unitWidth = width / (float)gridSize.x /3;
        unitHeight = height / (float)gridSize.y;

        if (points.Count < 2)                                       //always plot two vertices per point
        {
            return;
        }

        float angle = 0;
        

        for(int i = 0; i<points.Count-1; i++)
        {
            
            Vector2 point = points[i];
            Vector2 point2 = points[i + 1]; 

            if (i < points.Count - 1)
            {
                angle = GetAngle(points[i], points[i+1]) + 45f; 
            }

            DrawVerticesForPoint(point, point2, vh, angle); 
        }
        


        for (int i =0; i<points.Count-1; i++)                                                  //draw all triangles
        {
            int index = i *4; 
            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }
    }

    public float GetAngle(Vector2 me, Vector2 target)
    {
        return (float)(Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI));
    }

    void DrawVerticesForPoint(Vector2 point, Vector2 point2, VertexHelper vh, float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0,0,angle) * new Vector3(-thickness / 2, 0);         //vertices are placed and rotated so they keep the thickness
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0); 
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);
    }
    

    private void Update()
    {
        
        if (grid != null)
        {/*
            if (gridSize != grid.gridSize)
            {
                gridSize = grid.gridSize;
                SetVerticesDirty();
            }*/
        }
    }

  

    public void randomlyFillGraph()
    {
        points.Clear();
        for (int i = 0; i< 16; i++)
        {
            float randomNumber = Random.Range(-4f, 4f);

            points.Add(new Vector2(i, randomNumber+4));
        }
        this.SetAllDirty();
        
    }

    public List<Vector2> fillGraphFromList( List<Vector2> newPoints)
    {
        points.Clear();

        return points;
    }

    public List<Vector2> addNewPoint(List<Vector2> oldPoints)   //called from VitalSignsSimulation to add new simulated values
    {

         if(oldPoints.Count > pointCount)
         {
          
             oldPoints.RemoveAt(0);         //if graph is at the maximum length (i.e. long enough to fill the grid), the point on the left is deleted
         }
        
        Vector2 shift = new Vector2(1, 0);

        for (int i = 0; i < oldPoints.Count; i++)
        {
            oldPoints[i] -= shift;          //the points are moved in direction (-1, 0)

        }

        points = oldPoints;

        this.SetAllDirty();                 //the graph is repainted

        return points;
    }
}
