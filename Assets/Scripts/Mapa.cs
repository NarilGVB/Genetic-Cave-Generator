using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mapa {

    //Variables

    int[,] mapa;
    int alto, ancho;
    public string seed;
    int bordesPost = 0;
    int dentroPost = 0;
    int fueraPost = 0;
    int puentes = 0;
    double calidad;

    //Inicialización

    public Mapa()
    {
        alto = 50;
        ancho = 50;
        mapa = new int[alto, ancho];
    }

    public Mapa(int alto, int ancho)
    {
        this.alto = alto;
        this.ancho = ancho;
        mapa = new int[alto, ancho];
        this.ObtenerCalidad();
    }

    public Mapa(int alto, int ancho, int porcentajeRellenado)
    {
        this.alto = alto;
        this.ancho = ancho;
        GenerateRandomMap(porcentajeRellenado);
        this.ObtenerCalidad();
    }

    public Mapa(int[,] mapa)
    {
        alto = mapa.GetLength(0);
        ancho = mapa.GetLength(1);
        this.mapa = new int[alto, ancho];
        this.mapa = (int[,])mapa.Clone();
        this.ObtenerCalidad();
    }

    //Setters y Getters

    public int[,] GetMapa()
    {
        return mapa;
    }

    public int getPuntoMapa(int alto, int ancho)
    {
        return mapa[alto, ancho];
    }

    public void setPuntoMapa(int alto, int ancho, int punto)
    {
        mapa[alto, ancho] = punto;
    }
    
    public void setAlto(int alto)
    {
        this.alto = alto;
    }

    public void setAncho(int ancho)
    {
        this.ancho = ancho;
    }

    public void setDimensiones(int x, int y)
    {
        setAlto(y);
        setAncho(x);
    }

    public int getAlto()
    {
        return alto;
    }

    public int getAncho()
    {
        return ancho;
    }

    public double getCalidad()
    {
        return calidad;
    }

    //Manipulación del mapa

    public void GenerateRandomMap(int porcentajeRellenado)
    {
        mapa = new int[alto, ancho];
        seed = Time.realtimeSinceStartup.ToString();
        System.Random randomNum = new System.Random(seed.GetHashCode());
        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                mapa[i, j] = randomNum.Next(0, 100) < porcentajeRellenado ? 1 : 0;
            }
        }
        for (int i = 0; i < 5; i++)
        {
            CorregirMapa();
        }
        AnalizarMapa();
        ObtenerCalidad();
    }

    public void CorregirMapa()
    {
        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                int numVecinos = Vecinos(i, j);
                if (numVecinos > 4)
                {
                    mapa[i, j] = 1;
                }
                else if (numVecinos < 4)
                    mapa[i, j] = 0;
            }
        }
    }

    void AnalizarMapa()
    {
        dentroPost = 0;
        fueraPost = 0;
        bordesPost = 0;
        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                int numVecinos = Vecinos(i, j);
                if (mapa[i, j] == 1 && numVecinos == 8)
                    dentroPost++;
                else if (mapa[i, j] == 0)
                {
                    fueraPost++;
                    if (numVecinos == 2)
                        puentes++;
                }
                else if (numVecinos != 0 && numVecinos != 8 && mapa[i, j] == 1)
                    bordesPost++;
            }
        }
    }

    public double ObtenerCalidad()
    {
        AnalizarMapa();

        double d;
        double calidadDentro;
        double calidadFuera;
        double calidadBordes;

        d = Math.Abs(dentroPost - (alto * ancho * 0.65));
        calidadDentro = 1 / (1 + d);
        d = Math.Abs(fueraPost - (alto * ancho * 0.20));
        calidadFuera = 1 / (1 + d);
        d = Math.Abs(bordesPost - (alto * ancho * 0.15));
        calidadBordes = 1 / (1 + d);

        return calidad = calidadDentro + calidadFuera + calidadBordes - 1/puentes;
    }

    int Vecinos(int y, int x)
    {
        int numVecinos = 0;
        for (int i = y - 1; i <= y + 1; i++)
        {
            for (int j = x - 1; j <= x + 1; j++)
            {
                if (i >= 0 && j >= 0 && i < alto && j < ancho)
                {
                    if (i != y || j != x)
                    {
                        numVecinos += mapa[i, j];
                    }
                }
            }
        }
        return numVecinos;
    }
}
