import { IsString, IsEmail, IsInt, IsOptional } from "class-validator";

export class CreateMessageDto {
  @IsString()
  first_name!: string;

  @IsOptional()
  @IsString()
  last_name?: string;

  @IsOptional()
  @IsEmail()
  email?: string;

  @IsOptional()
  @IsString()
  phone?: string;

  @IsString()
  content!: string;

  @IsOptional()
  @IsInt()
  car_id?: number;
}
