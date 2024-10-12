LibTessDotNet for Unity [![Build Status](https://ci.appveyor.com/api/projects/status/ypuw4wca67vr5k8u?svg=true)](https://ci.appveyor.com/project/speps/libtessdotnet)
================================================================

### Goal
Provide a robust and fast tessellator (polygons with N vertices in the output) for Unity, also performing triangulation.

### Requirements
* .NET Standard 2.0 (see [here](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) for more information)
* Unity version 2019.1 or higher

### Features
* Tessellate arbitrary complex polygons
    - self-intersecting (see "star-intersect" sample)
    - with coincident vertices (see "clipper" sample)
    - advanced winding rules: even/odd, non zero, positive, negative, |winding| >= 2 (see "redbook-winding" sample)
* Custom input
    - Custom vertex attributes (e.g., UV coordinates) with merging callback
    - Force orientation of input contour (clockwise/counterclockwise, e.g., for GIS systems, see "force-winding" sample)
* Choice of output
    - polygons with N vertices (with N >= 3)
    - connected polygons (should work, though not extensively tested)
    - boundary only (to have a basic union of two contours)
* Handles polygons computed with [Clipper](http://www.angusj.com/delphi/clipper.php) - an open source freeware polygon clipping library
* Single/Double precision support

### Unity Integration Changes
This fork adds a **Unity Package Manifest** (`package.json`) to easily manage this library via the Unity **Package Manager**. By integrating **LibTessDotNet** as a Unity package, the library can be easily pulled into any Unity project and updated alongside other packages. It also includes all necessary **Unity `.meta` files** for seamless integration.

In addition to adding Unity-specific features, a significant amount of legacy content and files that are not required for Unity integration have been removed to streamline the package:
* Deleted **Build** folder: contained scripts and executables related to building the library outside of Unity, such as `BuildPackage.bat`, `PackageNuGet.bat`, etc.
* Removed **TessBed** and **TessExample** directories: these contained Windows Forms applications and benchmarking examples that are not relevant to Unity.
* Removed **.sln** and **.csproj** files: Unity does not use these for compilation, so they were unnecessary.
* Deleted **AppVeyor** configuration (`appveyor.yml`): not needed for Unity development.



### Build
Since this package is tailored for Unity, you can add it directly to your project via **Unity Package Manager** or manually copy the source files into your **Assets** folder.

To add via Unity Package Manager, add the following dependency to your project's `manifest.json` file:
```json
"com.caolanradford.libtessdotnet": "https://github.com/caolanradford/LibTessDotNet.git"
```

### Example
Here is an example script that demonstrates how to use LibTessDotNet within Unity. The example uses **LibTessDotNet** to tessellate a star-shaped polygon:

```csharp
using LibTessDotNet;
using UnityEngine;

public class TessellatorExample : MonoBehaviour
{
    void Start()
    {
        // Example input data in the form of a star that intersects itself.
        var inputData = new float[] { 0.0f, 3.0f, -1.0f, 0.0f, 1.6f, 1.9f, -1.6f, 1.9f, 1.0f, 0.0f };

        // Create an instance of the tessellator. Can be reused.
        var tess = new LibTessDotNet.Tess();

        // Construct the contour from inputData.
        int numPoints = inputData.Length / 2;
        var contour = new LibTessDotNet.ContourVertex[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            contour[i].Position = new LibTessDotNet.Vec3(inputData[i * 2], inputData[i * 2 + 1], 0);
        }
        tess.AddContour(contour, LibTessDotNet.ContourOrientation.Clockwise);

        // Tessellate using the EvenOdd winding rule, requesting triangles as output.
        tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3);

        // Output the generated triangles to the console.
        Debug.Log("Output triangles:");
        int numTriangles = tess.ElementCount;
        for (int i = 0; i < numTriangles; i++)
        {
            var v0 = tess.Vertices[tess.Elements[i * 3]].Position;
            var v1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
            var v2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;
            Debug.LogFormat("#{0} ({1:F1},{2:F1}) ({3:F1},{4:F1}) ({5:F1},{6:F1})", i, v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y);
        }
    }
}
```

### Notes
* When using `ElementType.BoundaryContours`, `Tess.Elements` will contain a list of ranges `[startVertexIndex, vertexCount]`. Those ranges are to be used with `Tess.Vertices`.

### TODO
* Implement pooling to reduce allocations when processing the same input repeatedly.
* Suggestions are always welcome! Feel free to open an issue or submit a pull request.

### License
**LibTessDotNet** is licensed under **SGI FREE SOFTWARE LICENSE B** (Version 2.0, Sept. 18, 2008). More information can be found in `LICENSE.txt`.

### Links
* [Reference implementation](http://oss.sgi.com/projects/ogl-sample) - the original SGI reference implementation
* [libtess2](https://github.com/memononen/libtess2) - Mikko Mononen cleaned up the original GLU tessellator
* [Poly2Tri](http://code.google.com/p/poly2tri/) - another triangulation library for .NET (other ports also available)
    - Does not support polygons from Clipper, more specifically vertices with the same coordinates (coincident)
* [Clipper](http://www.angusj.com/delphi/clipper.php) - an open source freeware polygon clipping library
### Acknowledgment
This package is based on the original [LibTessDotNet](https://github.com/speps/LibTessDotNet) by speps.
