import { prisma } from "../../config/prisma";
import bcrypt from "bcryptjs";
import jwt from "jsonwebtoken";

const JWT_SECRET = process.env.JWT_SECRET || "super_secret_key";

export class AuthService {
  async login(username: string, password: string) {
    const user = await prisma.users.findUnique({
      where: { username },
      include: { user_roles: true },
    });

    console.log(user);
    if (!user) throw new Error("Invalid credentials");

    const valid = await bcrypt.compare(password, user.password_hash);
    if (!valid) throw new Error("Invalid credentials");

    const token = jwt.sign(
      {
        id: user.id,
        username: user.username,
        role: user.user_roles.name,
      },
      JWT_SECRET,
      { expiresIn: "2h" }
    );

    return {
      message: "Login successful",
      token,
      user: {
        id: user.id,
        username: user.username,
        full_name: user.full_name,
        email: user.email,
        role: user.user_roles.name,
      },
    };
  }

  verifyToken(token: string) {
    try {
      return jwt.verify(token, JWT_SECRET);
    } catch {
      throw new Error("Invalid or expired token");
    }
  }
}
