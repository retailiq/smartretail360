
import type { NextConfig } from "next";

const isProd = process.env.NODE_ENV === "production";

const nextConfig: NextConfig = {
  ...(isProd && {
    basePath: "/client",
    assetPrefix: "/client",
  }),
  output: "standalone",
};

export default nextConfig;