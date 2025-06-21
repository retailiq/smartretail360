import Image from "next/image";

export default function Home() {
  return (
      <div className="flex items-center justify-center min-h-screen bg-background text-foreground p-6">
        <main className="flex flex-col gap-6 items-center text-center">
          <Image
              className="dark:invert"
              src="/logo_no_bg.png"
              alt="Next.js logo"
              width={180}
              height={38}
              priority
          />

          <h1 className="text-3xl font-semibold tracking-tight">
            Welcome to the Smart Retail 360 Frontend
          </h1>

          <p className="text-lg text-gray-600 dark:text-gray-300 max-w-md leading-relaxed">
            This system provides intelligent product recommendations, user insights, and data analytics services for
            merchants.
          </p>


          <span className="mt-4 text-sm sm:text-base font-medium text-blue-600 dark:text-blue-400 bg-blue-50 dark:bg-blue-900 px-3 py-1.5 rounded shadow-sm border border-blue-200 dark:border-blue-700">
            More features coming soon...
          </span>
        </main>
      </div>
  );
}
