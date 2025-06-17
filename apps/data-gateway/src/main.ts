import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { ConfigService } from '@nestjs/config';
import { SwaggerModule, DocumentBuilder } from '@nestjs/swagger';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);


  const config = new DocumentBuilder()
      .setTitle('SmartRetail Nest API')
      .setDescription('NestJS service API documentation')
      .setVersion('1.0')
      .addBearerAuth() // 如果你需要 JWT 鉴权
      .build();

  const document = SwaggerModule.createDocument(app, config);
  SwaggerModule.setup('swagger', app, document); // UI 地址: /swagger

  const configService = app.get(ConfigService);
  const port = configService.get<number>('PORT') ?? 50005;
  await app.listen(port);
  console.log(`Server is running on http://localhost:${port}`);
}
bootstrap();
