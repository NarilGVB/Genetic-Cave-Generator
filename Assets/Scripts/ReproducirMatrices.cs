using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReproducirMatrices : MonoBehaviour {

    Mapa geneticMap;
    public int alto;
    public int ancho;
    public int porcentajeRellenado;
    public int posMut;
    public int posCru;
    public GameObject inTile;
    public GameObject outTile;
    System.Random randomNum;
    public int geneticRounds;

    // Use this for initialization
    void Start () {
        geneticMap = new Mapa();
        geneticMap = GenerateGeneticMap();
        print("Final map quality: " + geneticMap.getCalidad());
	}

    Mapa GenerateGeneticMap()
    {
        Mapa map;
        Mapa[] mapas = new Mapa[4];
        for(int i = 0; i < 4; i++)
        {
            mapas[i] = new Mapa(alto, ancho, porcentajeRellenado);
            print(i+1 + " map quality: " + mapas[i].getCalidad());
        }
        map = new Mapa(alto, ancho, porcentajeRellenado);

        Mapa mapAux;

        string seed = Time.realtimeSinceStartup.ToString();
        randomNum = new System.Random(seed.GetHashCode());

        double calidadTotal = 0f;
        double[] calidadesPor100 = new double[4];

        for (int k = 0; k < geneticRounds; k++)
        {
            for (int i = 0; i < 4; i++)
            {
                calidadTotal += mapas[i].getCalidad();
            }
            for (int i = 0; i < 4; i++)
            {
                calidadesPor100[i] = (mapas[i].getCalidad() * 100) / calidadTotal;
                if (i > 0)
                    calidadesPor100[i] += calidadesPor100[i - 1];
            }

            Mapa[] hijos = new Mapa[4];

            for (int i = 0; i < 4; i+=2) {
                int numRepro = randomNum.Next(1, 100);
                int j;
                for (j = 0; calidadesPor100[j] <= numRepro && j < 3; j++) ;
                hijos[i] = new Mapa(mapas[j].GetMapa());

                numRepro = randomNum.Next(1, 100);
                for (j = 0; calidadesPor100[j] <= numRepro && j < 3; j++) ;
                hijos[i+1] = new Mapa(mapas[j].GetMapa());

                ReprodudirMapas(hijos[i], hijos[i+1]);
            }

            mapAux = mapas[0];
            for (int i = 1; i < 4; i++)
            {
                if (mapas[i].getCalidad() > mapAux.getCalidad())
                {
                    mapAux = mapas[i];
                }
            }

            for (int i=0; i < 4; i++)
            {
                for (int l = 0; l < 3; l++)
                {
                    hijos[i].CorregirMapa();
                }
                hijos[i].ObtenerCalidad();
                mapas[i] = hijos[i];
                mapas[i].ObtenerCalidad();
            }

            int iAux=0;
            for (int i = 1; i < 4; i++)
            {
                if (mapas[i].getCalidad() < mapas[iAux].getCalidad())
                {
                    iAux = i;
                }
            }

            if(mapas[iAux].getCalidad() < mapAux.getCalidad())
            {
                mapas[iAux] = mapAux;
            }   
        }

        map = mapas[0];
        for (int i = 1; i < 4; i++)
        {
            if (mapas[i].getCalidad() > map.getCalidad())
            {
                map = mapas[i];
            }
        }

        DibujarMapa(map,inTile,outTile);
        return map;
    }

    void ReprodudirMapas(Mapa hijo1, Mapa hijo2)
    {
        int[,] mapaAux= new int[alto,ancho];
        int anchoR = randomNum.Next(0, ancho - 1);
        int altoR = randomNum.Next(0, alto - 1);

        if (randomNum.Next(1, 100) < posCru)
        {
            for(int i = 0; i < alto; i++)
            {
                for(int j = 0; j< ancho; j++)
                {
                    if (i > altoR || (i == altoR && j > anchoR))
                    {
                        mapaAux[i, j] = hijo1.getPuntoMapa(i, j);
                        hijo1.setPuntoMapa(i, j, hijo2.getPuntoMapa(i, j));
                        hijo2.setPuntoMapa(i, j, mapaAux[i, j]);
                    }
                }
            }
        }
        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                if(randomNum.Next(1, 100) < posMut)
                    hijo1.setPuntoMapa(i, j, randomNum.Next(0,1));
                if (randomNum.Next(1, 100) < posMut)
                    hijo2.setPuntoMapa(i, j, randomNum.Next(0, 1));
            }
        }
        hijo1.ObtenerCalidad();
        hijo2.ObtenerCalidad();
    }

    public void DibujarMapa(Mapa mapa,GameObject inTile, GameObject outTile)
    {
        Transform mapIm = new GameObject("Mapa").transform;
        for (int i = 0; i < alto; i++)
            for (int j = 0; j < ancho; j++)
            {
                GameObject toInstanciate = mapa.getPuntoMapa(i, j) == 1 ? inTile : outTile;
                GameObject intance = Instantiate(toInstanciate, new Vector3(i - (alto / 2), j - (ancho / 2), 1f), Quaternion.identity) as GameObject;
                intance.transform.SetParent(mapIm);
            }
    }
}