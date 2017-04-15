﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba_Arbol_B
{
    public class BTree<TLlave, T> where TLlave : IComparable<TLlave> where T : IComparable<T>
    {
        Fabrica<TLlave, T> fabricar;
        BNode<TLlave, T> root;
        string llaveNull = "####################################";
        int grado;

        public BTree(string FileName, int Grado)
        {
            fabricar = new Fabrica<TLlave, T>(FileName, Grado);
            root = null;
            this.grado = Grado;
        }

        public BTree(string FileName)
        {
            fabricar = new Fabrica<TLlave, T>(FileName);
            root = fabricar.TraerNodo(fabricar.ObtenerRaiz());
            this.grado = fabricar.ObtenerGrado();
        }

        //Metodos Insertar
        public void Insertar(TLlave key, T dato)
        {                 
            if (fabricar.Empty())
            {
                //Cuando lo que este insertando sea la raiz
                int raiz = fabricar.ObtenerPosicionLibre();
                fabricar.CambiarRaiz(raiz);
                fabricar.NodoDeFabrica();
                BNode<TLlave, T> nodo = fabricar.TraerNodo(raiz);
                nodo.Llaves[0] = key.ToString();
                nodo.Datos[0] = dato.ToString();
                fabricar.GuardarNodo(nodo.Informacion());

                //Modificar tamaño
                int tamanio = fabricar.ObtenerTamaño();
                tamanio++;
                fabricar.CambiarTamaño(tamanio);
            }
            else
            {
                //Cuando lo que estoy insertando no es la raiz :s    
                Insertando(key, dato);

                //Modificar tamaño
                int tamanio = fabricar.ObtenerTamaño();
                tamanio++;
                fabricar.CambiarTamaño(tamanio);
            }
        }

        private void Insertando(TLlave key, T dato)
        {
            int raiz = fabricar.ObtenerRaiz();
            BNode<TLlave, T> nodo = fabricar.TraerNodo(raiz);

            while(!EsHoja(nodo.Hijos))
            {
                int hijo = ADondeIr(nodo.Llaves, key);
                int traer = int.Parse(nodo.Hijos[hijo]);
                nodo = fabricar.TraerNodo(traer);
            }

            if (HayEspacio(nodo.Llaves))
            {
                nodo = InsertaEnNodoHoja(nodo, key, dato);
                fabricar.GuardarNodo(nodo.Informacion());
            }
            else
            {
                // Esta Lleno, entonces hay que crear nuevos nodos :s
                InsertarEnNodoLleno(nodo, key, dato);
            }
        }

        private BNode<TLlave, T> InsertaEnNodoHoja(BNode<TLlave, T> nodo, TLlave key, T dato)
        {
            for (int i = 0; i < nodo.Llaves.Length; i++)
            {
                //Si en caso el dato era mayor a todos los anteriores se inserta en la primera posicion
                //vacia que encuentre
                if (nodo.Llaves[i] == llaveNull)
                {
                    nodo.Llaves[i] = key.ToString();
                    nodo.Datos[i] = dato.ToString();
                    return nodo;
                }
                else
                {
                    if (!(key.ToString().CompareTo(nodo.Llaves[i]) > 0))
                    {
                        //Encuentra la siguiente posicion vacia para hacer el corrimiento de los datos
                        int vacio = EncontrarPosicionVacia(nodo.Llaves);

                        while (vacio != i)
                        {
                            //Mover Llaves
                            nodo.Llaves[vacio] = nodo.Llaves[vacio - 1];
                            //Mover Data
                            nodo.Datos[vacio] = nodo.Datos[vacio - 1];
                            vacio--;
                        }

                        nodo.Llaves[i] = key.ToString();
                        nodo.Datos[i] = dato.ToString();
                        return nodo;
                    }
                }
            }

            return nodo;

        }

        private void InsertarEnNodoLleno(BNode<TLlave, T> nodo, TLlave key, T dato)
        {
            string llave = key.ToString();
            string info = dato.ToString();
            BNode<TLlave, T> Padre;
            BNode<TLlave, T> Nuevo;
            BNode<TLlave, T> hijo1 = null;
            BNode<TLlave, T> hijo2 = null;
            int i, j;

            bool salir = false;

            do
            {
                if (nodo == null)
                {
                    int posicion = fabricar.ObtenerPosicionLibre();
                    fabricar.NodoDeFabrica();
                    fabricar.CambiarRaiz(posicion);
                    nodo = fabricar.TraerNodo(posicion);
                    root = nodo;

                    //Cambiar altura
                    int altura = fabricar.ObtenerAltura();
                    altura++;
                    fabricar.CambiarAltura(altura);
                }

                Padre = fabricar.TraerNodo(nodo.Padre);

                if (!HayEspacio(nodo.Llaves))
                {
                    //Overflow

                    List<string> lista = new List<string>();
                    List<string> listaDatos = new List<string>();
                    List<string> listapunt = new List<string>();

                    //Se crea el nodo Derecho
                    int libre = fabricar.ObtenerPosicionLibre();
                    fabricar.NodoDeFabrica();
                    Nuevo = fabricar.TraerNodo(libre);

                    //Se crean listas para encontrar el nodo que debe subir
                    i = 0;

                    while ((llave.CompareTo(nodo.Llaves[i]) > 0) && (i < grado -1))
                    {
                        lista.Insert(i, nodo.Llaves[i]);
                        listaDatos.Insert(i, nodo.Datos[i]);
                        listapunt.Insert(i, nodo.Hijos[i]);
                        i++;

                        if(i == nodo.Llaves.Count())
                        {
                            break;
                        }

                    }

                    lista.Insert(i, llave);
                    listaDatos.Insert(i, info);
                    
                    if(hijo1 != null)
                    {
                        listapunt.Insert(i, hijo1.Posicion.ToString("D11"));
                    }
                    else
                    {
                        listapunt.Insert(i, int.MinValue.ToString());
                    }

                    if(hijo2 != null)
                    {
                        listapunt.Insert(i + 1, hijo2.Posicion.ToString("D11"));
                    }
                    else
                    {
                        listapunt.Insert(i + 1,int.MinValue.ToString());
                    }


                    while (i < grado - 1)
                    {
                        lista.Insert(i + 1, nodo.Llaves[i]);
                        listaDatos.Insert(i + 1, nodo.Datos[i]);
                        listapunt.Insert(i + 2, nodo.Hijos[i + 1]);
                        i++;
                    }


                    //Se reconoce cual es el centro del nodo.

                    int centro = 0;

                    if ((lista.Count % 2) == 0)
                    {
                        //Grado Par
                        centro = (lista.Count / 2) - 1;
                    }
                    else
                    {
                        //Grado Impar
                        centro = lista.Count / 2;
                    }


                    //Dividir los nodos

                    //Nodo Izquierdo

                    for (j = 0; j < nodo.Llaves.Length; j++)
                    {
                        if (j < centro)
                        {
                            nodo.Llaves[j] = lista[j];
                            nodo.Datos[j] = listaDatos[j];
                            nodo.Hijos[j] = listapunt[j];
                        }
                        else
                        {
                            nodo.Llaves[j] = llaveNull;
                            nodo.Datos[j] = llaveNull;
                            nodo.Hijos[j] = int.MinValue.ToString();
                        }
                    }


                    //Para limpiar los demas nodos :s
                    nodo.Hijos[centro] = listapunt[centro];

                    if(centro != nodo.Hijos.Length)
                    {
                        for (int x = centro + 1; x < nodo.Hijos.Length; x++)
                        {
                            nodo.Hijos[x] = int.MinValue.ToString();
                        }
                    }

                    //Nodo Derecho

                    for (j = 0; j < Nuevo.Llaves.Length; j++)
                    {
                        if(grado % 2 == 0)
                        {
                            if (j <= centro)
                            {
                                Nuevo.Llaves[j] = lista[centro + j + 1];
                                Nuevo.Datos[j] = listaDatos[centro + j + 1];
                                Nuevo.Hijos[j] = listapunt[centro + j + 1];
                            }
                        }
                        else
                        {
                            if (j < centro)
                            {
                                Nuevo.Llaves[j] = lista[centro + j + 1];
                                Nuevo.Datos[j] = listaDatos[centro + j + 1];
                                Nuevo.Hijos[j] = listapunt[centro + j + 1];
                            }
                        }
                       
                    }

                    Nuevo.Hijos[EspaciosUsados(Nuevo.Llaves)] = listapunt[grado];

                    //Hay que corregir los hijos en cada nodo creado

                    for (j = 0; j <= EspaciosUsados(nodo.Llaves); j++)
                    {
                        BNode<TLlave, T> temp = fabricar.TraerNodo(int.Parse(nodo.Hijos[j]));
                        if(temp != null)
                        {
                            temp.Padre = nodo.Posicion;
                            fabricar.GuardarNodo(temp.Informacion());
                        }
                    }
                        //if (nodo->Hijos[j])
                        //    (nodo->Hijos[j])->Padre = nodo;

                    for (j = 0; j <= EspaciosUsados(Nuevo.Llaves); j++)
                    {
                        BNode<TLlave, T> temp = fabricar.TraerNodo(int.Parse(Nuevo.Hijos[j]));
                        if(temp != null)
                        {
                            temp.Padre = Nuevo.Posicion;
                            fabricar.GuardarNodo(temp.Informacion());
                        }
                    }

                    llave = lista[centro];
                    info = listaDatos[centro];
                    hijo1 = nodo;
                    hijo2 = Nuevo;
                    fabricar.GuardarNodo(hijo1.Informacion());
                    fabricar.GuardarNodo(hijo2.Informacion());
                    nodo = Padre;
                }
                else
                {
                    //Continua insertando
                    //Inserta la clave en su lugar

                    i = 0;

                    if(EspaciosUsados(nodo.Llaves) > 0)
                    {
                        int llavesUsadas = EspaciosUsados(nodo.Llaves);

                        while((i < llavesUsadas) && (llave.CompareTo(nodo.Llaves[i]) > 0)) i++;
                        {
                            for (j =llavesUsadas; j > i; j--)
                            {
                                nodo.Llaves[j] = nodo.Llaves[j - 1];
                                nodo.Datos[j] = nodo.Datos[j - 1];
                            }
  
                            for (j = llavesUsadas + 1; j > i; j--)
                            {
                                nodo.Hijos[j] = nodo.Hijos[j - 1];
                            }                         
                        }
                    }

                    nodo.Llaves[i] = llave;
                    nodo.Datos[i] = info;

                    //Asignar hijo 1 al nodo
                    if(hijo1 != null)
                    {
                        nodo.Hijos[i] = int.Parse(hijo1.Posicion.ToString()).ToString("D11");
                        hijo1.Padre = nodo.Posicion;
                    }
                    else
                    {
                        nodo.Hijos[i] = int.MinValue.ToString();
                    }
                    
                    //Asignar hijo 2 al nodo
                    if(hijo2 != null)
                    {
                        nodo.Hijos[i + 1] = int.Parse(hijo2.Posicion.ToString()).ToString("D11");
                        hijo2.Padre = nodo.Posicion;
                    }
                    else
                    {
                        nodo.Hijos[i + 1] = int.MinValue.ToString();
                    }

                    //Guardamos los datos
                    fabricar.GuardarNodo(nodo.Informacion());
                    fabricar.GuardarNodo(hijo1.Informacion());
                    fabricar.GuardarNodo(hijo2.Informacion());

                    salir = true;               
                }

            }
            while (!salir);
        }


        //Metodos de Busqueda

        public int Buscar(TLlave key, T dato)
        {
            BNode<TLlave, T> nodo;

            if(fabricar.Empty())
            {
                return int.MinValue;
            }
            else
            {
                int raiz = fabricar.ObtenerRaiz();
                nodo = fabricar.TraerNodo(raiz);

                //Inicia la busqueda desde la raiz.
                while(nodo != null)
                {
                    for(int i = 0; i < nodo.Datos.Length; i++)
                    {
                        if(nodo.Datos[i] == dato.ToString())
                        {
                            return nodo.Posicion;
                        }
                    }

                    int siguiente = int.Parse(nodo.Hijos[ADondeIr(nodo.Llaves, key)]);
                    nodo = fabricar.TraerNodo(siguiente);
                }

                return int.MinValue;
            }
        }


        private bool HayEspacio(string[] nodo)
        {
            for (int i = 0; i < nodo.Length; i++)
            {
                if (nodo[i] == llaveNull)
                {
                    return true;
                }
            }

            return false;
        }

        private int EspaciosUsados(string[] llaves)
        {
            int espacios = 0;

            for (int i = 0; i < llaves.Length; i++)
            {
                if (llaves[i] != llaveNull)
                {
                    espacios++;
                }
            }

            return espacios;
        }

        /// <summary>
        /// Posicion vacia de las llaves.
        /// </summary>
        /// <param name="nodo"></param>
        /// <returns></returns>
        private int EncontrarPosicionVacia(string[] llaves)
        {
            //Encontrar la posicion vacia mas cercana
            for (int x = 0; x < llaves.Length; x++)
            {
                if (llaves[x] == llaveNull)
                {
                    return x;
                }
            }

            return 0;
        }

        private bool EsHoja(string[] nodo)
        {
            for(int x = 0; x < nodo.Length; x++)
            {              
                if(nodo[x] != int.MinValue.ToString())
                {
                    return false;
                }
            }

            return true;
        }

        private int ADondeIr(string[] nodo, TLlave key) 
        {
            //El arreglo que se recibe es el arreglo de llaves.

            int hijo = grado - 1;

            for(int i = 0; i < grado -1; i++)
            {
                if(nodo[i] != llaveNull)
                {
                    if (!(key.ToString().CompareTo(nodo[i]) > 0))
                    {
                        hijo = i;
                        return hijo;
                    }
                }
                else
                {
                    hijo = i;
                    return hijo;
                }
            }

            return hijo;
        }



    }
}
