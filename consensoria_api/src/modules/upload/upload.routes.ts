import express from "express";
import multer from "multer";

import fs from "fs";
import cloudinary from "../../config/cloudinary";

const router = express.Router();
const upload = multer({ dest: "tmp/" });

// 🔹 Subida múltiple
router.post("/multi", upload.array("files", 10), async (req, res) => {
  try {
    console.log("📁 Archivos recibidos para subida múltiple:");
    if (!req.files || !Array.isArray(req.files))
      return res.status(400).json({ message: "No se recibieron archivos." });

    const uploads = [];
    for (const file of req.files) {
      const result = await cloudinary.uploader.upload((file as any).path, {
        folder: "consensoria_autos",
      });
      fs.unlinkSync((file as any).path);
      uploads.push({
        url: result.secure_url,
        public_id: result.public_id,
      });
    }
    console.log("Archivos añadidos:");

    return res.json({ images: uploads });
  } catch (err: any) {
    console.error("❌ Error al subir múltiples imágenes:", err);
    return res
      .status(500)
      .json({ message: "Error interno", error: err.message });
  }
});

export default router;
