using System;
using System.IO;
using System.Net;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
    public struct Vector
    {
        public float x, y, z;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public static Vector operator +(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }
        
        public static Vector operator -(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }
        
        
        public static Vector operator *(Vector v, float f)
        {
            return new Vector(v.x * f, v.y * f, v.z * f);
        }
        
        public static Vector operator /(Vector v, float f)
        {
            return new Vector(v.x / f, v.y / f, v.z / f);
        }

        public static Vector Max(Vector v, Vector u)
        {
            return new Vector(MathF.Max(v.x, u.x), MathF.Max(v.y, u.y), MathF.Max(v.z, u.z));
        }
        
        public static Vector Min(Vector v, Vector u)
        {
            return new Vector(MathF.Min(v.x, u.x), MathF.Min(v.y, u.y), MathF.Min(v.z, u.z));
        }
        
    }
    
    class Program
    {
        static Vector[] vertices = new Vector[]
        {
            new Vector(-.1f, -.1f),
            new Vector(.1f, -.1f),
            new Vector(0f, .1f),
            // new Vector( .4f, .4f),
            // new Vector(.6f, .4f),
            // new Vector(.5f, .6f)
        };

        const int vertexSize = 3;
        
        static void Main(string[] args) {
            
            var window = CreatWindow();
            LoadTriangleIntoBuffer();
            CreateShaderProgram();

            // engine rendering loop
            var direction = new Vector(0.0001f, 0.0001f, 0);
            var multiplyer = 0.9999f;
            float scale = 1f;
            
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents(); // react to window changes (position etc.)
                ClearScreen();
                Render(window);

                for (var i = 0; i < vertices.Length; i ++)
                {
                    vertices[i] += direction;
                }

                
                //Find center of triangle

                var min = vertices[0];
                for (int i = 0; i < vertices.Length; i++)
                {
                    min = Vector.Min(min, vertices[i]);
                }

                var max = vertices[0];
                for (int i = 0; i < vertices.Length; i++)
                {
                    min = Vector.Max(max, vertices[i]);
                }

                var center = (min + max) / 2;
                
                // Move all vertices towards center
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] -= center;
                }
                
                // Scale
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] *= multiplyer;
                }
                
                scale *= multiplyer;

                if (scale <= 0.5f) multiplyer = 1.0001f;
                else if (scale >= 1) multiplyer = 0.9999f;
                

                // Move all vertices back center
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] += center;
                }

                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].x  >= 1 && direction.x > 0 || vertices[i].x <= -1 && direction.x < 0)
                    {
                        direction.x *= -1;
                        break;
                    }
                }

                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].y  >= 1 && direction.y > 0 || vertices[i].y <= -1 && direction.y < 0)
                    {
                        direction.y *= -1;
                        break;
                    }
                }
                UppdateBuffer();
            } 
        }

        

        private static void Render(Window window)
        {
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            Glfw.SwapBuffers(window);
            //glFlush();
        }

        private static void ClearScreen()
        {
            glClearColor(0, 0, 0, 1);
            glClear(GL_COLOR_BUFFER_BIT);
        }

        private static Window CreatWindow()
        {
            // initialize and configure
            Glfw.Init();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.Decorated, true);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, Constants.True);
            Glfw.WindowHint(Hint.Doublebuffer, Constants.True);

            // create and launch a window
            var window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }

        private static unsafe void LoadTriangleIntoBuffer()
        {

            // load the vertices into a buffer
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            fixed (Vector* vertex = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vector) * vertices.Length, vertex, GL_STATIC_DRAW);
            }

            glVertexAttribPointer(0, vertexSize, GL_FLOAT, false, sizeof(Vector), NULL);
            

            glEnableVertexAttribArray(0);
        }

        static unsafe void UppdateBuffer()
        {
            fixed (Vector* vertex = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vector) * vertices.Length, vertex, GL_STATIC_DRAW);
            }
        }

        private static void CreateShaderProgram()
        {
            // create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("Shader/screen-coordinates.vert"));
            glCompileShader(vertexShader);

            // create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("Shader/green.frag"));
            glCompileShader(fragmentShader);

            // create shader program - rendering pipeline
            var program = glCreateProgram();
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragmentShader);
            glLinkProgram(program);
            glUseProgram(program);
        }
    }
}