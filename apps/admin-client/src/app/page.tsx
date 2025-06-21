import Image from "next/image";

export default function Home() {
  return (
      <div className="flex items-center justify-center min-h-screen bg-background text-foreground p-6">
        <main className="flex flex-col gap-6 items-center text-center">
          <Image
              className="dark:invert"
              src="/logo_no_bg.png"
              alt="SmartRetail360 Admin Logo"
              width={180}
              height={38}
              priority
          />

          <h1 className="text-3xl font-semibold tracking-tight">
            Welcome to the Smart Retail 360 Admin Panel
          </h1>

          <p className="text-lg text-gray-600 dark:text-gray-300 max-w-md leading-relaxed">
            This portal enables administrators to manage platform settings, user permissions, and system-wide analytics for Smart Retail 360.
          </p>

          <span className="mt-4 text-sm sm:text-base font-medium text-blue-600 dark:text-blue-400 bg-blue-50 dark:bg-blue-900 px-3 py-1.5 rounded shadow-sm border border-blue-200 dark:border-blue-700">
            More features coming soon...
          </span>
        </main>
      </div>
  );
}