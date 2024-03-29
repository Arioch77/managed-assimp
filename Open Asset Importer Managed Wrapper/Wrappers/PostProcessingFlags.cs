﻿/*
 * Copyright (C) 2011 by John Hardy
 * 
 * This file is part of The Managed Assimp Wrapper.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to dea
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * If you would like to use The Managed Assimp Wrapper under another license, 
 * contact John Hardy at john at highwire-dtc dot com.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 * Many thanks to the people at Assimp (assimp.sourceforge.net) 
 * and SlimDX (slimdx.org) for their fantastic work without which, this would not have been
 * possible.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assimp.ManagedAssimp
{
    /// <summary>
    /// Lists all provided post processing steps.
    /// Post processing steps are various algorithms to improve the output scene.
    /// They compute additional data, help identify invalid data and optimize the 3d
    /// scene for a particular purpose. Post processing is one of the key concept of
    /// Assimp. If the various flags seem confusing to you, simply try one of the
    /// presets and check out whether the results fit to your requirements.
    /// </summary>
    /// <author>Alexander Gessler (aramis_acg@users.sourceforge.net)</author>
    /// <author>John Hardy</author>
    /// <date>20 July 2009</date>
    public abstract class PostProcessingFlags
    {
        /// <summary>
        /// Declares some handy post processing presets.
        /// </summary>
        /// <author>Alexander Gessler (aramis_acg@users.sourceforge.net)</author>
        public abstract class Preset
        {
            /// <summary>
            /// Simple set of flags which handle DirectX transformations. 
            /// </summary>
            public static uint Target_DirectX = MakeLeftHanded | FlipUVs | FlipWindingOrder;//FlipUVs;//FlipWindingOrder | MakeLeftHanded | FlipUVs;

            /// <summary>
            /// Simple post processing preset targeting fast loading for use in real time apps.
            /// Applications would want to use this preset to load models on end-user
            /// PCs, maybe even for direct use in game. That's not absolutely what
            /// Assimp was designed for but who cares ..?
            /// If you're using DirectX, don't forget to combine this value with the
            /// #ConvertToLeftHanded step. If you don't support UV
            /// transformations in your application apply the
            /// #TransformUVCoords step, too.
            /// </summary>
		    public static uint TargetRealtime_Fast = CalcTangentSpace | GenNormals | JoinIdenticalVertices | Triangulate | GenUVCoords | SortByPType | 0;

            /// <summary>
            /// Default post processing configuration targeting real time rendering
            /// Unlike TargetRealtime_Fast this predefined configuration performs
            /// some extra optimizations to improve rendering speed and to minimize
            /// memory usage. It could be a good choice for a level editor
            /// environment where import speed is not so important.
            /// If you're using DirectX, don't forget to combine this value with the
            /// #ConvertToLeftHanded step. If you don't support UV
            /// transformations in your application apply the
            /// #TransformUVCoords step, too.
            /// </summary>
            public static uint TargetRealtime_Quality = CalcTangentSpace | Target_DirectX
                    | GenSmoothNormals | JoinIdenticalVertices | ImproveCacheLocality | LimitBoneWeights
				    | RemoveRedundantMaterials | SplitLargeMeshes | Triangulate | GenUVCoords
                    | SortByPType | /*FindDegenerates | FindInvalidData |*/ FixInfacingNormals | OptimizeMeshes | OptimizeGraph | 0;

            /// <summary>
            /// Default post processing configuration targeting real time rendering.
            /// This preset enables almost every optimization step to achieve
            /// perfectly optimized data. It's your choice for level editor
            /// environments where import speed doesn't care.
            /// If you're using DirectX, don't forget to combine this value with the
            /// ConvertToLeftHanded step. If you don't support UV transformations in
            /// your application, apply the TransformUVCoords step too.
            /// </summary>
            public static uint TargetRealtime_MaxQuality = TargetRealtime_Quality | FindInstances | ValidateDataStructure | /*OptimizeMeshes | OptimizeGraph |*/ 0;
	    }

        /// <summary>
        /// Calculates the tangents and bitangents (aka 'binormals') for the imported meshes.
        /// Does nothing if a mesh does not have normals. You might want this post
        /// processing step to be executed if you plan to use tangent space
        /// calculations such as normal mapping applied to the meshes. There's a
        /// separate setting, <tt>#AI_CONFIG_PP_CT_MAX_SMOOTHING_ANGLE</tt>, which
        /// allows you to specify a maximum smoothing angle for the algorithm.
        /// However, usually you'll want to let the default value. Thanks.
        /// </summary>
        public static uint CalcTangentSpace = 0x1;

        /// <summary>
        /// Identifies and joins identical vertex data sets within all imported
        /// meshes.
        /// After this step is run each mesh does contain only unique vertices
        /// anymore, so a vertex is possibly used by multiple faces. You usually want
        /// to use this post processing step. If your application deals with indexed
        /// geometry, this step is compulsory or you'll just waste rendering time.
        /// <b>If this flag is not specified</b>, no vertices are referenced by more
        /// than one face and <b>no index buffer is required</b> for rendering.
        /// </summary>
        public static uint JoinIdenticalVertices = 0x2;

        /// <summary>
        /// Converts all the imported data to a left-handed coordinate space.
        /// By default the data is returned in a right-handed coordinate space which
        /// for example OpenGL prefers. In this space, +X points to the right, +Z
        /// points towards the viewer and and +Y points upwards. In the DirectX
        /// coordinate space +X points to the right, +Y points upwards and +Z points
        /// away from the viewer.
        /// You'll probably want to consider this flag if you use Direct3D for
        /// rendering. The #ConvertToLeftHanded flag supersedes this
        /// setting and boundles all conversions typically required for D3D-based
        /// applications.
        /// </summary>
        public static uint MakeLeftHanded = 0x4;

        /// <summary>
        /// Triangulates all faces.
        /// Originally the imported mesh data might contain faces with more than 3
        /// indices. For rendering you'll usually want all faces to be triangles.
        /// This post processing step splits up all higher faces to triangles. Lines
        /// and points are *not* modified!. If you want 'triangles only' with no
        /// other kinds of primitives, try the following:
        /// <ul>
        /// <li>Specify both #Triangulate and #SortByPType</li>
        /// </li>Processing Assimp's output, ignore all point and line meshes</li>
        /// </ul>
        /// </summary>
        public static uint Triangulate = 0x8;

        /// <summary>
        /// Removes some parts of the data structure (animations, materials, light
        /// sources, cameras, textures, specific vertex components, ..).
        /// The components to be removed are specified in a separate configuration
        /// option, <tt>#AI_CONFIG_PP_RVC_FLAGS</tt>. This is quite useful if you
        /// don't need all parts of the output structure. Calling this step to remove
        /// unneeded stuff from the pipeline as early as possible results in better
        /// performance and a perfectly optimized output data structure. This step is
        /// also useful if you want to force Assimp to recompute normals or tangents.
        /// The corresponding steps don't recompute them if they're already there
        /// (loaded from the source asset). By using this step you can make sure they
        /// are NOT there :-)
        /// Consider the following case: a 3d model has been exported from a CAD
        /// application, it has per-face vertex colors. Vertex positions can't be
        /// shared, thus the #JoinIdenticalVertices step fails to optimize
        /// the data. Just because these nasty, little vertex colors. Most apps don't
        /// even process them so it's all for nothing. By using this step unneeded
        /// components are excluded as early as possible thus opening more room for
        /// internal optimizations.
        /// </summary>
        public static uint RemoveComponent = 0x10;

        /// <summary>
        /// Generates per-face normals for all meshes in the scene.
        /// This step is skipped if normals are already present in the source file.
        /// Model importers try to load them from the source file and many file
        /// formats provide support for them. Face normals are shared between all
        /// points of a single face. Thus a single point can have multiple normals
        /// forcing the library to duplicate vertices in some cases.
        /// #JoinIdenticalVertices is *senseless* then because there's
        /// nothing to be joined.
        /// This flag may not be specified together with #GenSmoothNormals.
        /// </summary>
        public static uint GenNormals = 0x20;

        /// <summary>
        /// Generates smooth per-vertex normals for all meshes in the scene.
        /// This step is skipped if normals are already present in the source file.
        /// Model importers try to load them from the source file and many file
        /// formats provide support for them.
        /// There the, <tt>#AI_CONFIG_PP_GSN_MAX_SMOOTHING_ANGLE</tt> configuration
        /// property which allows you to specify an angle maximum for the normal
        /// smoothing algorithm. Normals exceeding this limit are not smoothed,
        /// resulting in a a visually 'hard' seam between two faces. Using a decent
        /// angle here (e.g. 80°) results in very good visual appearance. Again, this
        /// doesn't apply if the source format provides proper normals.
        /// This flag may not be specified together with #GenNormals.
        /// </summary>
        public static uint GenSmoothNormals = 0x40;

        /// <summary>
        /// Splits large unbeatable meshes into smaller sub meshes.
        /// This is quite useful for real time rendering where the number of
        /// triangles which can be maximally processed in a single draw-call is
        /// usually limited by the video hardware. The maximum vertex buffer is
        /// usually limited too. Both requirements can be met with this step: you may
        /// specify both a triangle and vertex limit for a single mesh.
        /// The split limits can (and should!) be set through the
        /// <tt>#AI_CONFIG_PP_SLM_VERTEX_LIMIT</tt> and
        /// <tt>#AI_CONFIG_PP_SLM_TRIANGLE_LIMIT</tt> settings. The default values
        /// are <tt>#AI_SLM_DEFAULT_MAX_VERTICES</tt> and
        /// <tt>#AI_SLM_DEFAULT_MAX_TRIANGLES</tt>.
        /// Note that splitting is generally a time-consuming task, but not if
        /// there's nothing to split. The use of this step is recommended for most
        /// users.
        /// </summary>
        public static uint SplitLargeMeshes = 0x80;

        /// <summary>
        /// Removes the node graph and transforms all vertices by the absolute
        /// transformations of their host nodes. The output scene does still contain
        /// nodes but there is only a root node with children, each one referencing
        /// exactly only one mesh, each mesh referencing exactly one material. For
        /// rendering you can simply draw all meshes in order, you don't need to pay
        /// attention to local transformations and the node hierarchy. Animations are
        /// removed during this step. This step is intended for applications without
        /// a scene graph-like system.
        /// </summary>
        public static uint PreTransformVertices = 0x100;

        /// <summary>
        /// Limits the number of bones simultaneously affecting a single vertex to a
        /// maximum value.
        /// If any vertex is affected by more than that number of bones, the least
        /// important vertex weights are removed and the remaining vertex weights are
        /// renormalized so that the weights still sum up to 1. The default bone
        /// weight limit is 4 (defined as <tt>#AI_LMW_MAX_WEIGHTS</tt> in
        /// aiConfig.h), but you can use the <tt>#AI_CONFIG_PP_LBW_MAX_WEIGHTS</tt>
        /// setting to supply your own limit to the post processing step.
        /// If you intend to perform the skinning in hardware, this post processing
        /// step might be of interest for you.
        /// </summary>
        public static uint LimitBoneWeights = 0x200;

        /// <summary>
        /// Validates the imported scene data structure .This makes sure that all
        /// indices are valid, all animations and bones are linked correctly, all
        /// material references are correct .. etc.
        /// It is recommended to capture Assimp's log output if you use this flag, so
        /// you can easily find ot what's actually wrong if a file fails the
        /// validation. The validation is quite rude and will usually find *all*
        /// inconsistencies in the data structure ... plugin developers are
        /// recommended to use it to debug their loaders. There are two types of
        /// validation failures:
        /// <ul>
        /// <li>Error: There's something wrong with the imported data. Further post
        /// processing is not possible and the data is not usable at all. The import
        /// fails. <code>Importer::getErrorString()</code> retrieves the error
        /// string.</li>
        /// <li>Warning: There are some minor issues with the imported scene but
        /// further post processing and use of the data structure is still safe.
        /// Details about the issue are written to the log file and the
        /// <tt>Importer.SCENE_FLAGS_VALIDATION_WARNING</tt> scene flag is set</li>
        /// </ul>
        /// This post-processing step is not time-consuming at all. It's use is not
        /// compulsory but recommended.
        /// </summary>
        public static uint ValidateDataStructure = 0x400;

        /// <summary>
        /// Reorders triangles for better vertex cache locality.
        /// The step tries to improve the ACMR (average post-transform vertex cache
        /// miss ratio) for all meshes. The implementation runs in O(n) and is
        /// roughly based on the 'tipsify' algorithm.
        /// If you intend to render huge models in hardware, this step might be of
        /// interest for you. The <tt>#AI_CONFIG_PP_ICL_PTCACHE_SIZE</tt>
        /// configuration is provided to fine-tune the cache optimization for a
        /// particular target cache size. The default value is mostly fine.
        /// </summary>
        public static uint ImproveCacheLocality = 0x800;

        /// <summary>
        /// Searches for redundant/unreferenced materials and removes them.
        /// This is especially useful in combination with the
        /// <code>PretransformVertices</code> and <code>OptimizeMeshes</code> flags.
        /// Both join small meshes with equal characteristics, but they can't do
        /// their work if two meshes have different materials. Because several
        /// material settings are always lost during Assimp's import filters, (and
        /// because many exporters don't check for redundant materials), huge models
        /// often have materials which are are defined several times with exactly the
        /// same settings ..
        /// Several material settings not contributing to the appearance of a
        /// surface are ignored in all comparisons ... the material name is one of
        /// them. So, if you're passing additional information through the content
        /// pipeline (probably using using *magic* material names), don't specify
        /// this flag. Alternatively take a look at the
        /// <tt>#AI_CONFIG_PP_RRM_EXCLUDE_LIST</tt> setting.
        /// </summary>
        public static uint RemoveRedundantMaterials = 0x1000;

        /// <summary>
        /// This step tries to determine which meshes have normal vectors that are
        /// facing inwards. The algorithm is simple but effective: the bounding box
        /// of all vertices + their normals is compared against the volume of the
        /// bounding box of all vertices without their normals. This works well for
        /// most objects, problems might occur with planar surfaces. However, the
        /// step tries to filter such cases. The step inverts all in-facing normals.
        /// Generally it is recommended to enable this step, although the result is
        /// not always correct.
        /// </summary>
        public static uint FixInfacingNormals = 0x2000;

        /// <summary>
        /// This step splits meshes with more than one primitive type in homogeneous
        /// sub meshes.
        /// The step is executed directly after the triangulation step. After the
        /// step returns just *one* bit remains set in aiMesh::mPrimitiveTypes. This
        /// is especially useful for real-time rendering where point and line
        /// primitives are often ignored, or rendered separately. You can use the
        /// <tt>#AI_CONFIG_PP_SBP_REMOVE</tt> option to specify which primitive types
        /// you need. This can be used to easily exclude the rarely wanted lines and
        /// points from the import.
        /// </summary>
        public static uint SortByPType = 0x8000;

        /// <summary>
        /// This step searches all meshes for degenerated primitives and converts
        /// them to proper lines or points.
        /// A face is 'degenerated' if one or more of its points are identical. To
        /// have the degenerated stuff not only detected and collapsed but also
        /// removed, try one of the following procedures: <br>
        /// <b>1.</b> (if you support lines&points for rendering but don't want the
        /// degenerates)</br>
        /// <ul>
        /// <li>Specify the #FindDegenerates flag.</li>
        /// <li>Set the <tt>AI_CONFIG_PP_FD_REMOVE</tt> option to 1. This will cause
        /// the step to remove degenerated triangles from the import as soon as
        /// they're detected. They won't pass any further pipeline steps.</li>
        /// </ul>
        /// <br>
        /// <b>2.</b>(if you don't support lines&points at all ...)</br>
        /// <ul>
        /// <li>Specify the #FindDegenerates flag.</li>
        /// <li>Specify the #SortByPType flag. This moves line and point
        /// primitives to separate meshes.</li>
        /// <li>Set the <tt>AI_CONFIG_PP_SBP_REMOVE</tt> option to
        /// <code>aiPrimitiveType_POINTS | aiPrimitiveType_LINES</code> to enforce
        /// SortByPType to reject point and line meshes from the scene.</li>
        /// </ul>
        /// Degenerated polygons are not necessarily evil and that's why they're not
        /// removed by default. There are several file formats which don't support
        /// lines or points. Some exporters bypass the format specification and write
        /// them as degenerated triangle instead. Assimp can't guess for you, so you
        /// have to decide. YOU!
        /// </summary>
        public static uint FindDegenerates = 0x10000;

        /// <summary>
        /// This step searches all meshes for incorrect data such as all-zero normal
        /// vectors or invalid UV coordinates and removes them.
        /// This is especially useful for stuff like normals or tangents. Some
        /// exporters tend to write very strange stuff into their output files. This
        /// flag increases the chance that this is detected and repaired
        /// automatically.
        /// </summary>
        public static uint FindInvalidData = 0x20000;

        /// <summary>
        /// This step converts non-UV mappings (such as spherical or cylindrical
        /// mapping) to proper texture coordinate channels.
        /// Most applications will support UV mapping only so you will probably want
        /// to specify this step in every case. Note that Assimp is not always able
        /// to match the original mapping implementation of the 3d application which
        /// produced a model perfectly. It's always better to let max,maja,blender or
        /// whatever you're using compute the UV channels.
        /// </summary>
        public static uint GenUVCoords = 0x40000;

        /// <summary>
        /// This step applies per-texture UV transformations and bakes them to
        /// stand-alone texture coordinate channels.
        /// UV transformations are specified per-texture - see the
        /// <tt>#AI_MATKEY_UVTRANSFORM</tt> material key for more information. This
        /// step processes all textures with transformed input UV coordinates and
        /// generates new (pre-transformed) UV channel which replace the old channel.
        /// Most applications won't support UV transformations, so you will probably
        /// always want to request this step.
        /// </summary>
        public static uint TransformUVCoords = 0x80000;

        /// <summary>
        /// This step searches for duplicate meshes and replaces duplicates with
        /// references to the first mesh.
        /// This step takes a while, don't use it if you have no time for it. It's
        /// main purpose is to provide a workaround for the limitation that many
        /// export file formats don't support instanced meshes, so exporters need to
        /// duplicate meshes. This step removes the duplicates again. Please note
        /// that Assimp does currently not support per-node material assignment to
        /// meshes, which means that identical meshes with different materials are
        /// currently *not* joined, although this is planned for future versions.
        /// </summary>
        public static uint FindInstances = 0x100000;

        /// <summary>
        /// A post processing step to reduce the number of meshes in the scene.
        /// This is a very effective optimization and is recommended to be used
        /// together with <code>OptimizeGraph</code>, if possible. It is fully
        /// compatible with both <code>SplitLargeMeshes</code> and
        /// <code>SortByPType</code>.
        /// </summary>
        public static uint OptimizeMeshes = 0x200000;

        /// <summary>
        ///  Nodes with no animations, bones, lights or cameras assigned are collapsed
        /// and joined.
        /// Node names can be lost during this step. If you use special 'tag nodes'
        /// to pass additional information through your content pipeline, use the
        /// <tt>#AI_CONFIG_PP_OG_EXCLUDE_LIST<7tt> setting to specify a list of node
        ///  names you want to be kept. Nodes matching one of the names in this list won't
        ///  be touched or modified.
        ///  Use this flag with caution. Most simple files will be collapsed to a
        ///  single node, complex hierarchies are usually completely lost. That's not
        ///  the right choice for editor environments, but probably a very effective
        ///  optimization if you just want to get the model data, convert it to your
        ///  own format and render it as fast as possible.
        ///  This flag is designed to be used with #OptimizeMeshes for best
        ///  results.
        /// 'Crappy' scenes with thousands of extremely small meshes packed in
        /// deeply nested nodes exist for almost all file formats.
        /// <code>OptimizeMeshes</code> in combination with
        /// <code>OptimizeGraph</code> usually fixes them all and makes them
        ///  beatable.
        /// </summary>
        public static uint OptimizeGraph = 0x400000;

        /// <summary>
        /// This step flips all UV coordinates along the y-axis and adjusts material
        /// settings and bitangents accordingly. <br>
        /// <b>Output UV coordinate system:</b> <code>
        /// 0y|0y ---------- 1x|0y
        /// |                 |
        /// |                 |
        /// |                 |
        /// 0x|1y ---------- 1x|1y
        /// </code>
        /// You'll probably want to consider this flag if you use Direct3D for
        /// rendering. The #ConvertToLeftHanded flag supersedes this
        /// setting and includes all conversions typically required for D3D-based
        /// applications.
        /// </summary>
        public static uint FlipUVs = 0x800000;

        /// <summary>
        /// This step adjusts the output face winding order to be clockwise.
        /// The default face winding order is counter-clockwise. <br>
        /// <b>Output face order:</b>
        /// <code> x2
        ///       x0 x1
        /// </code>
        /// You'll probably want to consider this flag if you use Direct3D for
        /// rendering. The #ConvertToLeftHanded flag supersedes this
        /// setting and includes all conversions typically required for D3D-based
        /// applications.
        /// </summary>
        public static uint FlipWindingOrder = 0x1000000;
    }
}
